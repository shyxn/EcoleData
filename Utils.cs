using OxyPlot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

        public static OxyColor GetSerieColor(int floorNb, string value)
        {
            ColorFamily floorFamily = GetColorFamily(floorNb);
            return OxyColor.Parse(value switch
            {
                "Température" => floorFamily.DarkColor,
                "Humidité" => floorFamily.NormalColor,
                "Point de rosée" => floorFamily.LightColor,
            });
        }

        public static Brush GetFaceColor(int floorNb, Func<ColorFamily, string> familyProperty) => (Brush)new BrushConverter().ConvertFromString(familyProperty(GetColorFamily(floorNb)));
        
        private static ColorFamily GetColorFamily(int floorNb) => ColorFamilies.Where(family => family.FloorNumber == floorNb).First();
            
        public static List<ColorFamily> ColorFamilies = new()
        {
            new ColorFamily()
            {
                FloorNumber = 0,
                DarkColor = "#20637F",
                NormalColor = "#287CA1",
                LightColor = "#3198C4"
            },
            new ColorFamily()
            {
                FloorNumber = 1,
                DarkColor = "#475F74",
                NormalColor = "#587591",
                LightColor = "#6E8BA6"
            },
            new ColorFamily()
            {
                FloorNumber = 2,
                DarkColor = "#3F7570",
                NormalColor = "#4E908B",
                LightColor = "#61AAA4"
            },
            new ColorFamily()
            {
                FloorNumber = 3,
                DarkColor = "#38886F",
                NormalColor = "#45AA8A",
                LightColor = "#5CBC9E"
            },
            new ColorFamily()
            {
                FloorNumber = 4,
                DarkColor = "#73A349",
                NormalColor = "#92BD6D",
                LightColor = "#A2C781"
            },
            new ColorFamily()
            {
                FloorNumber = 5,
                DarkColor = "#F4B711",
                NormalColor = "#F7C94F",
                LightColor = "#F8D26A"
            },
            new ColorFamily()
            {
                FloorNumber = 6,
                DarkColor = "#F58A10",
                NormalColor = "#F8A74D",
                LightColor = "#F9B669"
            },
            new ColorFamily()
            {
                FloorNumber = 7,
                DarkColor = "#F7590A",
                NormalColor = "#F9854A",
                LightColor = "#FA9663"
            },
            new ColorFamily()
            {
                FloorNumber = 8,
                DarkColor = "#D7540E",
                NormalColor = "#F2722B",
                LightColor = "#F4864C"
            },
            new ColorFamily()
            {
                FloorNumber = 9,
                DarkColor = "#F80509",
                NormalColor = "#FB4145",
                LightColor = "#FC5E60"
            },
        };
    }
}
