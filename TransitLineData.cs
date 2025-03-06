using Newtonsoft.Json;
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
    public class TransitLineData
    {
        public string VehicleType { get; set; }
        public long ID { get; set; }
        public string NWRType { get; set; }
        public string Name { get; set; }
        public string ShortenedName { get; set; }
        public double Latitude { get; set; } //used for cached Data
        public double Longitude { get; set; } //used for cached Data


        public TransitLineData(string vehicleType, string NWRType, long ID, string name, double latitude, double longitude)
        {
            this.VehicleType = vehicleType;
            this.ID = ID;
            this.Name = name;
            this.NWRType = NWRType;
            this.ShortenedName = Regex.Replace(Name, @":\s*.*?=>\s*", ": ");
            this.Latitude = Math.Round(latitude, 3); //we round to 3rd decimal = ~100m
            this.Longitude = Math.Round(longitude, 3); //we round to 3rd decimal = varies in length but between 110 - 50 m in most inhabited areas
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
