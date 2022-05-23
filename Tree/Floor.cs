using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoleData.Tree
{
    public class Floor
    {
        public Dictionary<string,  Location> Locations { get; set; }
        public Floor()
        {
        }
        public Dictionary<string, Location> SetLocations(string floorPath)
        {
            List<string> locationNames = Utils.GetFoldersNames(floorPath).ToList();
            Locations = locationNames.ToDictionary(
                x => x,
                x => new Location() { filePath = x });
            
            // On s'arrête ici - pas d'autre méthode car on est au niveau le plus bas
            return Locations;
        }
    }
}
