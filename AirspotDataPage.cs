using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    internal class AirSpotDataPage
    {
        public int pageID;
        public List<long> timestamps;
        public List<int> CO2values;
        public bool finishedPage;

        public AirSpotDataPage(byte[] data)
        {
            this.timestamps = new List<long>();
            this.CO2values = new List<int>();

            parseData(data);
        }

        private void parseData(byte[] data)
        {
            finishedPage = true;

            int offset = 4;
            for (int i = 0; i < 16; i++)
            {
                uint timestamp = (uint)(
                    (data[offset] << 24) |
                    (data[offset + 1] << 16) |
                    (data[offset + 2] << 8) |
                    data[offset + 3]);
                timestamps.Add(timestamp);

                if (timestamp == 0xFFFFFFFF) //unfinished pages have a FFFFFFFF
                {
                    finishedPage = false;
                }

                offset += 4;
                //ushort co2 = BitConverter.ToUInt16(response, offset);
                byte high = data[offset];
                byte low = data[offset + 1];
                int co2 = (high << 8) | low;
                CO2values.Add(co2);
                offset += 2;

                ushort unused = BitConverter.ToUInt16(data, offset); // Likely unused
                offset += 2;

                DateTime time = new DateTime(2000, 1, 1).AddSeconds(timestamp);
                //Console.WriteLine($"Entry {i}: Time={time}, CO2={co2} ppm");
            }
            byte pageNumberHigh = data[offset];
            byte pageNumberLow = data[offset + 1];
            int pageNumber = (pageNumberHigh << 8) | pageNumberLow;
            pageID = pageNumber;
            //Console.WriteLine("pageNumber:" + pageNumber);
        }
    }
}
