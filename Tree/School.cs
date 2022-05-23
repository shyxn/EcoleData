using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoleData.Tree
{
    public class School
    {
        public Dictionary<string, Floor> Floors { get; set; }
        public School()
        {

        }
        public School SetFloors(string schoolPath)
        {
            string[] floorNames = Utils.GetFoldersNames(schoolPath);

            Floors = floorNames.ToDictionary(name => name, name => {
                Floor floor = new Floor();
                // Pour chaque étage, on cherche ensuite, à un niveau plus bas, à configurer le dictionnaire Locations...
                floor.SetLocations(schoolPath + "\\" + name);
                return floor;
            });
            return this;
        }
    }
}
