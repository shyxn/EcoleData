using OxyPlot;
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

        public static OxyColor GetSerieColor(string floorNb)
        {
            return Convert.ToInt32(floorNb) switch
            {
                0 => OxyColors.Blue,
                1 => OxyColors.Red,
                2 => OxyColors.Green,
                3 => OxyColors.Gray,
                4 => OxyColors.Purple,
                5 => OxyColors.BlueViolet,
                6 => OxyColors.Beige,
                7 => OxyColors.Cyan,
                8 => OxyColors.Chocolate,
                9 => OxyColors.Khaki
            };
        }
    }
}
