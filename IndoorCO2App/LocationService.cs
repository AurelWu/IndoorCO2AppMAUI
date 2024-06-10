using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    internal abstract class LocationService
    {
        internal abstract bool IsGpsEnabled();
        internal abstract Task<bool> ShowEnableGpsDialogAsync();
    }
}
