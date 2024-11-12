using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public string ShortenedName;


        public TransitLineData(string vehicleType, string NWRType, long ID, string name)
        {
            this.VehicleType = vehicleType;
            this.ID = ID;
            this.Name = name;
            this.NWRType = NWRType;
            this.ShortenedName = Regex.Replace(Name, @":\s*.*?=>\s*", ": ");
        }


        public override string ToString()
        {
            string ShortenedName = Regex.Replace(Name, @":\s*.*?=>\s*", ": ");
            if(MainPage.MainPageSingleton.favouredLocations.Contains(NWRType+"_"+ID.ToString()))
            {
                return $"★ {ShortenedName} ";
            }
            else return $"{ShortenedName} ";
        }
    }
}
