using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TfsRestServices
{
    public class TfsJsonService
    {
        public async Task<List<TfsJsonProject>> GetProjects(TfsConnectionDetails connection, string projectCollection)
        {
            return await GetFromTfs<TfsJsonProject>(connection, $"{projectCollection}/_apis/projects", new Dictionary<string, string>());
        }

        public async Task<List<TfsJsonBuild>> GetBuildsStatuses(TfsConnectionDetails connection, Dictionary<string, string> queryParams, string projectCollection)
        {
            return await GetFromTfs<TfsJsonBuild>(connection, $"{projectCollection}/_apis/build/builds", queryParams);
        }

        public async Task<List<TfsJsonBuildDefinition>> GetBuildDefinitions(TfsConnectionDetails connection, string projectCollection, string project)
        {
            return await GetFromTfs<TfsJsonBuildDefinition>(connection, $"{projectCollection}/{project}/_apis/build/definitions", new Dictionary<string, string>());
        }

        public async Task<List<TfsJsonComment>> GetComments(TfsJsonBuild tfsJsonBuild, TfsConnectionDetails connection, string projectCollection)
        {
            var comments = await GetFromTfs<TfsJsonComment>(connection, $"{projectCollection}/_apis/build/builds/{tfsJsonBuild.Id}/changes",
                        new Dictionary<string, string>());
            return comments;
        }

        public async Task<List<TfsJsonProjectCollection>> GetProjectCollections(TfsConnectionDetails connection)
        {
            return await GetFromTfs<TfsJsonProjectCollection>(connection, "_apis/projectcollections", new Dictionary<string, string>());
        }

        private async Task<List<T>> GetFromTfs<T>(TfsConnectionDetails connection, string api, Dictionary<string, string> queryParams)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.AllowAutoRedirect = true;
                handler.Credentials = connection.AsNetworkConnection();

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = connection.GetBaseAddress();
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    var credentialsBase64Encoded = connection.Base64EncodeCredentials();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialsBase64Encoded);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var queryParamsAsString = string.Concat(queryParams.Select(i => "&" + i.Key + "=" + i.Value));
                    var response = await httpClient.GetAsync(api + "?api-version=2.0" + queryParamsAsString);
                    if (response.IsSuccessStatusCode)
                    {
                        var buildDefinitionsStr = await response.Content.ReadAsStringAsync();
                        // this is embarrasingly fragile, need to find a better way to determine if invalid credentials
                        var invalidCredentials = !buildDefinitionsStr.StartsWith("{") && buildDefinitionsStr.Contains("Sign In");
                        if (invalidCredentials)
                        {
                            throw new InvalidCredentialException("The credentials entered appear to be incorrect");
                        }
                        var jsonWrapper = JsonConvert.DeserializeObject<TfsJsonWrapper<T>>(buildDefinitionsStr);
                        return jsonWrapper.Value;
                    }
                    else
                    {
                        throw new HttpRequestException("Unexpected status code returned" + response.StatusCode);
                    }
                }
            }
        }
    }
}
