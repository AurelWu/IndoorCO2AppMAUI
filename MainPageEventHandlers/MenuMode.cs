using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    [System.Flags]
    internal enum MenuMode
    {
        Standard = 1,
        Recording = 2,
        ManualRecording = 4,
        TransportRecording = 8,
        TransportSelection = 16,

    }
}
