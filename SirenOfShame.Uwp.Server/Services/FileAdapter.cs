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
        public async Task AppendAllText(string location, string contents)
        {
            var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(location);
            await FileIO.AppendTextAsync(storageFile, contents);
        }

        public async Task<bool> Exists(string location)
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            return files.Any(i => i.Name == location);
        }

        public async Task<IEnumerable<string>> ReadAllLines(string location)
        {
            var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(location);
            return await FileIO.ReadLinesAsync(storageFile);
        }

        public async Task<IEnumerable<string>> GetFiles(string fileExtension)
        {
            var readOnlyList = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            return readOnlyList
                .Where(i => i.Name.Split('.').LastOrDefault() == fileExtension)
                .Select(i => i.Name);
        }
    }
}
