using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoleData
{
    public static class Utils
    {
        public static string[] GetFoldersNames(string foldersPath) => 
            Directory.GetDirectories(foldersPath, "*", SearchOption.TopDirectoryOnly)
            .Select(path => path = path.Substring(foldersPath.Length + 1)).ToArray();
        public static string[] GetFilesNames(string foldersPath) =>
            Directory.GetFiles(foldersPath, "*.csv", SearchOption.TopDirectoryOnly)
            .Select(path => path = path.Substring(foldersPath.Length + 1)).ToArray();
    }
}
