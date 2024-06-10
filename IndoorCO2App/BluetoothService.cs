using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    internal abstract class BluetoothService
    {
        internal abstract bool IsBluetoothEnabled();
        internal abstract Task<bool> ShowEnableBluetoothDialogAsync();
    }
}
