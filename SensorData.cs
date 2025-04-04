﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;



namespace IndoorCO2App_Multiplatform
{
    internal class SensorData
    {
        internal int CO2ppm;
        internal long relativeTimeStamp;
        internal DateTime dateTime;
        //we could also track temperature... but meh lets keep it small and simple
        //maybe we also grab user Lat / Lon at that point to ensure they stayed in the building?

        public SensorData(int ppm, long relativeTime, DateTime dateTime)
        {
            this.CO2ppm = ppm;
            this.relativeTimeStamp = relativeTime;
            this.dateTime = dateTime;
        }

        public override string ToString()
        {
            return CO2ppm.ToString();
        }
    }
}
