using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Lib.Watcher;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class FileAdapter : IFileAdapter
    {
        public Task AppendAllText(string location, string contents)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string location)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> ReadAllLines(string location)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetFiles(string fileExtension)
        {
            throw new NotImplementedException();
        }
    }
}
