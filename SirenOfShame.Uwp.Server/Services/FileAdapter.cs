using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using SirenOfShame.Lib.Watcher;

namespace SirenOfShame.Uwp.Server.Services
{
    public class FileAdapter : IFileAdapter
    {
        private async Task<StorageFile> GetFileAsync(string location)
        {
            return await KnownFolders.DocumentsLibrary.GetFileAsync(location);
        }

        private async Task CreateFileAsync(string location)
        {
            await KnownFolders.DocumentsLibrary.CreateFileAsync(location);
        }

        private async Task<IEnumerable<StorageFile>> GetFilesAsync()
        {
            return await KnownFolders.DocumentsLibrary.GetFilesAsync();
        }

        private async Task AppendTextAsync(string contents, StorageFile storageFile)
        {
            await FileIO.AppendTextAsync(storageFile, contents);
        }

        private static async Task<string> ReadTextAsync(StorageFile storageFile)
        {
            return await FileIO.ReadTextAsync(storageFile);
        }

        private static async Task<IList<string>> ReadLinesAsync(StorageFile storageFile)
        {
            return await FileIO.ReadLinesAsync(storageFile);
        }

        private static async Task WriteTextAsync(string contents, StorageFile storageFile)
        {
            await FileIO.WriteTextAsync(storageFile, contents);
        }

        public async Task AppendAllText(string location, string contents)
        {
            if (!await Exists(location))
            {
                await CreateFileAsync(location);
            }
            var storageFile = await GetFileAsync(location);
            await AppendTextAsync(contents, storageFile);
        }

        public async Task WriteTextAsync(string location, string contents)
        {
            if (!await Exists(location))
            {
                await CreateFileAsync(location);
            }
            var storageFile = await GetFileAsync(location);
            await WriteTextAsync(contents, storageFile);
        }

        public async Task<bool> Exists(string location)
        {
            var files = await GetFilesAsync();
            return files.Any(i => i.Name == location);
        }

        public async Task<IEnumerable<string>> ReadAllLines(string location)
        {
            var storageFile = await GetFileAsync(location);
            return await ReadLinesAsync(storageFile);
        }

        public async Task<string> ReadTextAsync(string location)
        {
            var storageFile = await GetFileAsync(location);
            return await ReadTextAsync(storageFile);
        }

        public async Task<IEnumerable<string>> GetFiles(string fileExtension)
        {
            var readOnlyList = await GetFilesAsync();
            return readOnlyList
                .Where(i => i.Name.Split('.').LastOrDefault() == fileExtension)
                .Select(i => i.Name);
        }

        public async Task DeleteAsync(string location)
        {
            var storageFile = await GetFileAsync(location);
            await storageFile.DeleteAsync();
        }
    }
}
