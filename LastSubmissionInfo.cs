using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    internal class LastSubmissionInfo

    {
        public string locationName;
        public long startTime;


        public LastSubmissionInfo(string locationName, long startTime)
        {
            this.locationName = locationName;
            this.startTime = startTime;
        }
    }
}
