﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Search;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;

namespace SirenOfShame.Uwp.Maintenance.Services
{
    internal class BundleService
    {
        private readonly ILog _log;
        private readonly CertificatePinningHttpClientFactory _httpClientFactory;
        private const string BASE_URL = "https://sirenofshame.com/DeployMe/";

        public BundleService(ILog log, CertificatePinningHttpClientFactory httpClientFactory)
        {
            _log = log;
            _httpClientFactory = httpClientFactory;
        }

        public async Task TryUpdate(Bundle bundle)
        {
            await _log.Debug("Checking for new version of " + bundle.Id);

            var installedPackage = FindPackageById(bundle.Id);
            if (installedPackage == null)
            {
                await _log.Debug("No package found with id " + bundle.Id);
                return;
            }

            var installedVersion = ToVersion(installedPackage.Id.Version);
            var serverVersion = bundle.Version;
            if (serverVersion <= installedVersion)
            {
                await _log.Debug("No work to do here, " + bundle.Id + " has the same version: " + installedVersion);
                return;
            }

            await _log.Info($"Starting download to upgrade {bundle.FileName} from {installedVersion} to {serverVersion}");
            var storageFile = await DownloadAppx(bundle.FileName);
            await _log.Debug("Download complete.  Installing update.");
            var result = await UpdatePackage(installedPackage, storageFile);
            await _log.Info($"Upgrade of {bundle.Id} result: {result}");
        }

        private Version ToVersion(PackageVersion installedVersion)
        {
            return new Version(installedVersion.Major, installedVersion.Minor, installedVersion.Build, installedVersion.Revision);
        }

        private static Package FindPackageById(string bundleId)
        {
            var packageManager = new PackageManager();
            var packages = packageManager.FindPackages();
            return packages.FirstOrDefault(i => i.Id.Name == bundleId);
        }

        private async Task<string> UpdatePackage(Package installedPackage, StorageFile storageFile)
        {
            var localUri = new Uri(storageFile.Path);
            var packageManager = new PackageManager();
            DeploymentResult packageResult;
            try
            {
                packageResult = await packageManager.UpdatePackageAsync(localUri, new List<Uri>(),
                    DeploymentOptions.ForceApplicationShutdown);
            }
            catch (COMException)
            {
                await _log.Warn("Unable to update package " + installedPackage.Id.FullName + " normally, trying to uninstall and install it");
                await packageManager.RemovePackageAsync(installedPackage.Id.FullName, RemovalOptions.PreserveApplicationData);
                packageResult = await packageManager.AddPackageAsync(localUri, null, DeploymentOptions.None);
            }

            var result = string.IsNullOrEmpty(packageResult.ErrorText) ? "Success" : packageResult.ErrorText;
            return result;
        }

        private async Task<StorageFile> DownloadAppx(string appxbundle)
        {
            //BackgroundDownloader downloader = new BackgroundDownloader();
            //var destinationFile = System.IO.Path.GetTempPath();
            //var localUri = new Uri(destinationFile);
            //var storageFile = await StorageFile.CreateStreamedFileFromUriAsync("App1_1.0.0.0_arm_Debug.appxbundle", localUri, null);
            //var stuff = downloader.CreateDownload(uri, storageFile);

            Uri remoteUri = new Uri(BASE_URL + appxbundle);

            var storageFolder = await GetAppDataStorageFolder();
            var storageFile = await storageFolder.CreateFileAsync(appxbundle, CreationCollisionOption.GenerateUniqueName);
            return await _httpClientFactory.WithHttpClient(async httpClient =>
            {
                var buffer1 = await httpClient.GetBufferAsync(remoteUri);
                var buffer = buffer1.ToArray();
                using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                return storageFile;
            });
        }

        private static async Task<StorageFolder> GetAppDataStorageFolder()
        {
            var localAppData = Environment.SpecialFolder.LocalApplicationData;
            var appData = Environment.GetFolderPath(localAppData);
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(appData);
            return storageFolder;
        }

        internal async Task TryUpdate(List<Bundle> manifest, string appId)
        {
            var appToUpdate = manifest.FirstOrDefault(i => i.Id == appId);

            if (appToUpdate == null)
            {
                await _log.Debug("No bundle found with Id of " + appId);
                return;
            }

            await TryUpdate(appToUpdate);
        }

        public async Task DeleteDownloads()
        {
            var storageFolder = await GetAppDataStorageFolder();
            var files = await storageFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (file.Name.EndsWith(".appxbundle"))
                {
                    await file.DeleteAsync();
                }
            }
        }
    }
}