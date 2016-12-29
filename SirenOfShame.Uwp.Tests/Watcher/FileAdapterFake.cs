using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Lib.Watcher;

namespace SirenOfShame.Test.Unit.Watcher
{
    public class FileAdapterFake : IFileAdapter
    {
        public Dictionary<string, string> Files = new Dictionary<string, string>();

        public async Task<bool> Exists(string location)
        {
            await Task.Yield();
            return Files.ContainsKey(location);
        }

        public async Task AppendAllText(string location, string contents)
        {
            await Task.Yield();
            if (!Files.ContainsKey(location))
            {
                Files[location] = "";
            }
            Files[location] += contents;
        }

        public async Task<IEnumerable<string>> ReadAllLines(string location)
        {
            await Task.Yield();
            var result = Files[location]
                .TrimEnd('\n')
                .Split('\n')
                .Select(i => i.TrimEnd('\r'))
                .ToArray();
            return result;
        }

        public async Task<IEnumerable<string>> GetFiles(string fileExtension)
        {
            await Task.Yield();
            return Files
                .Where(i => i.Key.Split('.').LastOrDefault() == fileExtension)
                .Select(i => i.Value);
        }
    }
}
