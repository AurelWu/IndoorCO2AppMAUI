using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    internal class TransitLineData
    {
        public string VehicleType;
        public long ID;
        public string NWRType;
        public string Name;


        public TransitLineData(string vehicleType, string NWRType, long ID, string name)
        {
            this.VehicleType = vehicleType;
            this.ID = ID;
            this.Name = name;
            this.NWRType = NWRType;
        }


        public override string ToString()
        {
            string shortName = Regex.Replace(Name, @":\s*.*?=>\s*", ": ");
            return $"{shortName} ";
        }
    }
}
