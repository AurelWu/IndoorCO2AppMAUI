using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Android
{
    public class Haversine
    {
        public const double R = 6372.8; // Radius of the Earth in kilometers

        public static double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            Console.WriteLine($"lat1: {lat1} | lat2: {lat2} | lon1: {lon1} | lon2: {lon2}");

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);
            double dLat = lat2 - lat1;
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Pow(Math.Sin(dLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c * 1000; // Return distance in meters
        }

        private static double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
    }
}
