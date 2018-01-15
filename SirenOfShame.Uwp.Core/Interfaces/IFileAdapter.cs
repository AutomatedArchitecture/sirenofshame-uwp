using System.Collections.Generic;
using System.Threading.Tasks;

namespace SirenOfShame.Lib.Watcher
{
    public interface IFileAdapter
    {
        Task AppendAllText(string location, string contents);
        Task<bool> Exists(string location);
        Task<IEnumerable<string>> ReadAllLines(string location);
        Task<IEnumerable<string>> GetFiles(string fileExtension);
        Task<string> ReadTextAsync(string location);
        Task DeleteAsync(string location);
        Task WriteTextAsync(string location, string contents);
    }
}