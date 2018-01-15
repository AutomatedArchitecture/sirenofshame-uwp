using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Watcher.Exceptions;
using SirenOfShame.Uwp.Watcher.Helpers;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.HudsonServices
{
    public class HudsonService : ServiceBase
    {
        protected override async Task<XDocument> DownloadXml(string url, string userName, string password, string cookie = null)
        {
            WebClientXml webClientXml = new WebClientXml
            {
                // hudson/jenkins api's apparently always require base64 encoded credentials rather than basic auth
                AuthenticationType = AuthenticationTypeEnum.Base64EncodeInHeader
            };

            var downloadXml = await webClientXml.DownloadXml(url, userName, password, cookie);
            return downloadXml;
        }

        internal async Task<MyBuildDefinition[]> GetProjects(GetProjectsArgs getProjectsArgs)
        {
            var buildDefinitions = await GetProjects(getProjectsArgs.Url, getProjectsArgs.UserName, getProjectsArgs.Password);
            return buildDefinitions.Cast<MyBuildDefinition>().ToArray();
        }

        private static readonly ILog _log = MyLogManager.GetLog(typeof(HudsonService));

        public delegate void GetProjectsCompleteDelegate(HudsonBuildDefinition[] buildDefinitions);

        private async Task<HudsonBuildDefinition[]> GetProjects(string rootUrl, string userName, string password)
        {
            WebClientXml webClient = new WebClientXml
            {
                AuthenticationType = AuthenticationTypeEnum.Base64EncodeInHeader,
                UserName = userName,
                Password = password
            };
            rootUrl = GetRootUrl(rootUrl);

            var doc = await webClient.DownloadXml(rootUrl + "/api/xml", userName, password);
            if (doc.Root == null)
            {
                throw new Exception("No results returned");
            }
            HudsonBuildDefinition[] buildDefinitions = doc.Root
                .Elements("job")
                .Select(projectXml => new HudsonBuildDefinition(rootUrl, projectXml))
                .ToArray();
            return buildDefinitions;
        }

        private static Action<Exception> OnError(Action<Exception> onError)
        {
            return delegate (Exception ex)
            {
                _log.Error("Error connecting to server", ex);
                onError(ex);
            };
        }

        private string GetRootUrl(string rootUrl)
        {
            if (string.IsNullOrEmpty(rootUrl)) return null;
            rootUrl = rootUrl.TrimEnd('/');
            return rootUrl;
        }

        public IEnumerable<Task<HudsonBuildStatus>> GetBuildsStatuses(CiEntryPointSetting ciEntryPointSetting, IEnumerable<BuildDefinitionSetting> watchedBuildDefinitions)
        {
            var parallelResult = from buildDefinitionSetting in watchedBuildDefinitions
                                 select GetBuildStatus(buildDefinitionSetting, ciEntryPointSetting);
            return parallelResult.ToList();
        }

        private async Task<HudsonBuildStatus> GetBuildStatus(BuildDefinitionSetting buildDefinitionSetting, CiEntryPointSetting ciEntryPointSetting)
        {
            string userName = ciEntryPointSetting.UserName;
            string password = ciEntryPointSetting.GetPassword();
            var rootUrl = GetRootUrl(ciEntryPointSetting.Url);
            var treatUnstableAsSuccess = ciEntryPointSetting.TreatUnstableAsSuccess;

            string url = rootUrl + "/job/" + buildDefinitionSetting.Id + "/api/xml";
            try
            {
                XDocument doc = await DownloadXml(url, userName, password);
                if (doc.Root == null)
                {
                    return new HudsonBuildStatus(null, buildDefinitionSetting, "Could not get project status", treatUnstableAsSuccess);
                }
                var lastBuildElem = doc.Root.Element("lastBuild");
                if (lastBuildElem == null)
                {
                    return new HudsonBuildStatus(null, buildDefinitionSetting, "No project builds found", treatUnstableAsSuccess);
                }
                var buildNumber = lastBuildElem.ElementValueOrDefault("number");
                var buildUrl = rootUrl + "/job/" + buildDefinitionSetting.Id + "/" + buildNumber;
                if (string.IsNullOrWhiteSpace(buildUrl)) throw new Exception("Could not get build url");
                buildUrl += "/api/xml";
                doc = await DownloadXml(buildUrl, userName, password);
                if (doc.Root == null) throw new Exception("Could not get project build status");
                return GetBuildStatusAndCommentsFromXDocument(buildDefinitionSetting, doc, treatUnstableAsSuccess);
            }
            catch (SosException ex)
            {
                if (ex.Message.ToLower().Contains("not_found"))
                {
                    throw new BuildDefinitionNotFoundException(buildDefinitionSetting);
                }
                throw;
            }
        }

        private HudsonBuildStatus GetBuildStatusAndCommentsFromXDocument(BuildDefinitionSetting buildDefinitionSetting, XDocument doc, bool treatUnstableAsSuccess)
        {
            return new HudsonBuildStatus(doc, buildDefinitionSetting, null, treatUnstableAsSuccess);
        }
    }
}
