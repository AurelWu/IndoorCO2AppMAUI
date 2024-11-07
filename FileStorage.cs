using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;

namespace IndoorCO2App_Multiplatform
{
    public static class FileStorage
    {
        private static string FilePath => Path.Combine(FileSystem.Current.AppDataDirectory, "FavouriteLocations.json");

        public static async Task SaveHashSetAsync(HashSet<string> hashSet)
        {
            var json = JsonConvert.SerializeObject(hashSet);
            // Use FileSystem API for writing
            bool existsBefore = File.Exists(FilePath);
            await File.WriteAllTextAsync(FilePath, json);
            bool existsAfter = File.Exists(FilePath);
            Console.WriteLine(existsBefore + "_" + existsAfter);
        }

        //public static async Task<HashSet<string>> LoadHashSetAsync()
        //{
        //    try
        //    {
        //        // Attempt to open the file
        //        using (var stream = await FileSystem.Current.OpenAppPackageFileAsync(FilePath))
        //        using (var reader = new StreamReader(stream))
        //        {
        //            var json = await reader.ReadToEndAsync();
        //            return JsonConvert.DeserializeObject<HashSet<string>>(json) ?? new HashSet<string>();
        //        }
        //    }
        //    catch (FileNotFoundException)
        //    {
        //        // Return an empty HashSet if file doesn't exist
        //        return new HashSet<string>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error reading file: {ex.Message}");
        //        return new HashSet<string>();
        //    }
        //}

        public static async Task<HashSet<string>> LoadHashSetAsync()
        {
            try
            {
                // Check if the file exists in AppDataDirectory
                if (!File.Exists(FilePath))
                {
                    // If the file doesn't exist, return an empty HashSet
                    return new HashSet<string>();
                }

                // Open and read the file
                using (var stream = File.OpenRead(FilePath))
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<HashSet<string>>(json) ?? new HashSet<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return new HashSet<string>();
            }
        }
    }
}
