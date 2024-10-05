using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    public interface ILocationService
    {
        public bool IsGpsEnabled();
        public Task<bool> ShowEnableGpsDialogAsync();
    }
}
