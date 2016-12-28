using System;
using System.Collections.Generic;
using System.IO;

namespace SirenOfShame.Lib.Watcher
{
    public class FileAdapter
    {
        public virtual bool Exists(string location)
        {
            throw new NotImplementedException();
            //return File.Exists(location);
        }

        public virtual void AppendAllText(string location, string contents)
        {
            throw new NotImplementedException();
            //File.AppendAllText(location, contents);
        }

        public virtual IEnumerable<string> ReadAllLines(string location)
        {
            throw new NotImplementedException();
            //try
            //{
            //    return !File.Exists(location) ? new string[] {} : File.ReadAllLines(location);
            //}
            //catch (FileNotFoundException)
            //{
            //    return new string[] { };
            //}
        }
    }
}
