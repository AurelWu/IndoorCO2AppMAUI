
/// <summary>
/// IMPORTANT: Put this file in root of Application
/// </summary>
public partial class BluetoothPermissions : Permissions.BasePlatformPermission
{
    internal static async Task<bool> CheckBluetoothPermissionStatus()
    {
        try
        {
            var requestStatus = await new BluetoothPermissions().CheckStatusAsync();
            return requestStatus == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    internal static async Task<bool> RequestBluetoothAccess()
    {
        try
        {
            var requestStatus = await new BluetoothPermissions().RequestAsync();
            return requestStatus == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            
            return false;
        }
    }
}