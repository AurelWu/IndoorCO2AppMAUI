﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App
{
    public static class RecoveryData
    {
        public static long startTime;
        public static long timeOfLastUpdate;
        public static long locationID;
        public static string locationType; //nwr
        public static string locationName;
        public static double locationLat;
        public static double locationLon;
        public const string prefRecoveryStartTime = "recovery_startTime";
        public const string prefRecoveryTimeOfLastUpdate = "recovery_timeOfLastUpdate";
        public const string prefRecoveryLocationID = "recovery_locationID";
        public const string prefRecoveryLocationType = "recovery_locationType";
        public const string prefRecoveryLocationName = "recovery_locationName";
        public const string prefRecoveryLocationLat = "recovery_locationLat";
        public const string prefRecoveryLocationLon = "recovery_locationLon";        

        public static void ReadFromPreferences()
        {
            startTime =  long.Parse(Preferences.Get(prefRecoveryStartTime, "0"));
            timeOfLastUpdate = long.Parse(Preferences.Get(prefRecoveryTimeOfLastUpdate, "0"));
            locationID = Preferences.Get(prefRecoveryLocationID,0);
            locationType = Preferences.Get(prefRecoveryLocationType,"");
            locationName = Preferences.Get(prefRecoveryLocationName, "");
            locationLat = Preferences.Get(prefRecoveryLocationLat, 0);
            locationLon = Preferences.Get(prefRecoveryLocationLon, 0);
        }

        public static void WriteToPreferences()
        {
            Preferences.Set(prefRecoveryStartTime, startTime.ToString());
            Preferences.Set(prefRecoveryTimeOfLastUpdate , timeOfLastUpdate.ToString());
            Preferences.Set(prefRecoveryLocationID , locationID);
            Preferences.Set(prefRecoveryLocationType , locationType);
            Preferences.Set(prefRecoveryLocationName , locationName);
            Preferences.Set(prefRecoveryLocationLat , locationLat);
            Preferences.Set(prefRecoveryLocationLon , locationLon);
        }

        public static void ResetRecoveryData()
        {
            Preferences.Set(prefRecoveryStartTime, "0");
            Preferences.Set(prefRecoveryTimeOfLastUpdate, "0");
            Preferences.Set(prefRecoveryLocationID, 0);
            Preferences.Set(prefRecoveryLocationType, "");
            Preferences.Set(prefRecoveryLocationName, "");
            Preferences.Set(prefRecoveryLocationLat, 0);
            Preferences.Set(prefRecoveryLocationLon, 0);
        }
    }

    }
