using System;
using System.Security.Cryptography;
using System.Text;

namespace IndoorCO2App_Multiplatform
{
    public static class EncryptionManager
    {
        public static string EncryptString(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to hexadecimal string
                StringBuilder hexString = new StringBuilder();
                foreach (byte b in hash)
                {
                    hexString.AppendFormat("{0:x2}", b);
                }
                return hexString.ToString();
            }
        }
    }
}