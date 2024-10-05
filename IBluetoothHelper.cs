
namespace IndoorCO2App_Multiplatform
{

    public interface IBluetoothHelper
    {
        public abstract bool CheckStatus();
        public abstract Task<PermissionStatus> RequestAsync();


        public abstract void EnsureDeclared();


        public abstract bool HasPermissionInManifest(string permission);


        public abstract void RequestBluetoothEnable();

        public abstract bool CheckIfBTEnabled();
    }
}














