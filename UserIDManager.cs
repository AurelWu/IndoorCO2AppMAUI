using System;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace IndoorCO2App_Android
{
    public static class UserIDManager
    {
        private const string PrefUserID = "user_id";
        //private const string PrefName = "IndoorDataCollectorUserID";

        public static string GetUserID()
        {
            var userID = Preferences.Get(PrefUserID, null);

            // If no user ID is stored, generate a new one and save it
            if (userID == null)
            {
                userID = GenerateRandomID();
                SaveUserID(userID);
            }

            return userID;
        }

        private static string GenerateRandomID()
        {
            return Guid.NewGuid().ToString();
        }

        private static void SaveUserID(string userID)
        {
            Preferences.Set(PrefUserID, userID);
        }

        public static string GetEncryptedID(string deviceID)
        {
            string userID = GetUserID();
            string concatenatedString = userID + deviceID;
            return EncryptionManager.EncryptString(concatenatedString);
        }
    }
}