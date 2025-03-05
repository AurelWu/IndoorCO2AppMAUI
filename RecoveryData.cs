using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorCO2App_Multiplatform
{
    public static class RecoveryData
    {
        public static bool windowsOpen;
        public static bool ventilation;
        public static string customNotes;

        public static string CO2MonitorType;
        public static string recordingMode;
        public static long startTime;
        public static long timeOfLastUpdate;

        public static long transportOriginID;
        public static string transportOriginType; //nwr
        public static string transportOriginName;
        public static long transportLineID;
        public static string transportLineType; //nwr
        public static string transportLineName;

        public static long locationID;
        public static string locationType; //nwr
        public static string locationName;
        public static double locationLat;
        public static double locationLon;
        public static string sensorValuesString;
        public static List<int> sensorValues = new List<int>();
        public const string prefRecoveryRecordingMode = "recovery_recordingMode";
        public const string prefRecoveryStartTime = "recovery_startTime";
        public const string prefRecoveryTimeOfLastUpdate = "recovery_timeOfLastUpdate";

        public const string prefRecoveryWindows = "recovery_windowsOpen";
        public const string prefRecoveryVentilation = "recovery_ventilation";
        public const string prefRecoveryCustomNotes = "recovery_customNotes";

        public const string prefRecoverytransportOriginID = "recovery_transportOriginID";
        public const string prefRecoverytransportOriginType = "recovery_transportOriginType";
        public const string prefRecoverytransportOriginName = "recovery_transportOriginName";

        public const string prefRecoverytransportLineID = "recovery_transportLineID";
        public const string prefRecoverytransportLineType = "recovery_transportLineType";
        public const string prefRecoverytransportLineName = "recovery_transportLineName";

        public const string prefRecoveryLocationID = "recovery_locationID";
        public const string prefRecoveryLocationType = "recovery_locationType";
        public const string prefRecoveryLocationName = "recovery_locationName";
        public const string prefRecoveryLocationLat = "recovery_locationLat";
        public const string prefRecoveryLocationLon = "recovery_locationLon";
        public const string prefCO2MonitorType = "recovery_CO2MonitorType";
        public const string prefRecoverySensorValues = "recovery_sensorValues";       //TODO: Add recorded values for sensors without history


        public static void ReadFromPreferences()
        {
            try
            {
                recordingMode = Preferences.Get(prefRecoveryRecordingMode, "");
                startTime = long.Parse(Preferences.Get(prefRecoveryStartTime, "0"));
                timeOfLastUpdate = long.Parse(Preferences.Get(prefRecoveryTimeOfLastUpdate, "0"));
                locationID = long.Parse(Preferences.Get(prefRecoveryLocationID, "0"));
                locationType = Preferences.Get(prefRecoveryLocationType, "");
                locationName = Preferences.Get(prefRecoveryLocationName, "");
                locationLat = double.Parse(Preferences.Get(prefRecoveryLocationLat, "0"));
                locationLon = double.Parse(Preferences.Get(prefRecoveryLocationLon, "0"));

                transportLineID = long.Parse(Preferences.Get(prefRecoverytransportLineID, "0"));
                transportLineName = Preferences.Get(prefRecoverytransportLineName, "");
                transportLineType = Preferences.Get(prefRecoverytransportLineType, "");

                transportOriginID = long.Parse(Preferences.Get(prefRecoverytransportOriginID, "0"));
                transportOriginName = Preferences.Get(prefRecoverytransportOriginName, "");
                transportOriginType = Preferences.Get(prefRecoverytransportOriginType, "");
                CO2MonitorType = Preferences.Get(prefCO2MonitorType,"");
                customNotes = Preferences.Get(prefRecoveryCustomNotes, "");
                windowsOpen = Preferences.Get(prefRecoveryWindows, false);
                ventilation = Preferences.Get(prefRecoveryVentilation, false);
                MainPage.MainPageSingleton._CheckBoxVentilation.IsChecked = ventilation;
                MainPage.MainPageSingleton._CheckBoxDoorsWindows.IsChecked = windowsOpen;
                MainPage.MainPageSingleton._NotesEditor.Text = customNotes;
            }
            catch (Exception e)
            {
                //Logger.circularBuffer.Add("Error Retrieving stored Data");
                string x = e.ToString();
                ResetRecoveryData();
            }
            
        }

        public static void WriteToPreferences()
        {
            try
            {

            
            Preferences.Set(prefRecoveryRecordingMode, recordingMode);
            Preferences.Set(prefRecoveryStartTime, startTime.ToString());
            Preferences.Set(prefRecoveryTimeOfLastUpdate, timeOfLastUpdate.ToString());
            Preferences.Set(prefRecoveryLocationID, locationID.ToString());
            Preferences.Set(prefRecoveryLocationType, locationType);
            Preferences.Set(prefRecoveryLocationName, locationName);
            Preferences.Set(prefRecoveryLocationLat, locationLat.ToString());
            Preferences.Set(prefRecoveryLocationLon, locationLon.ToString());

            Preferences.Set(prefRecoverytransportOriginType, transportOriginType);
            Preferences.Set(prefRecoverytransportOriginName, transportOriginName);
            Preferences.Set(prefRecoverytransportOriginID, transportOriginID.ToString());

            Preferences.Set(prefRecoverytransportLineType, transportLineType);
            Preferences.Set(prefRecoverytransportLineName, transportLineName);
            Preferences.Set(prefRecoverytransportLineID, transportLineID.ToString());
            Preferences.Set(prefCO2MonitorType, CO2MonitorType);
            Preferences.Set(prefRecoveryVentilation, MainPage.MainPageSingleton._CheckBoxVentilation.IsChecked);
            Preferences.Set(prefRecoveryWindows, MainPage.MainPageSingleton._CheckBoxDoorsWindows.IsChecked);
            Preferences.Set(prefRecoveryCustomNotes, MainPage.MainPageSingleton._NotesEditor.Text);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Error writing recovery data to preferences", false);
            }
        }

        public static void ResetRecoveryData()
        {
            Preferences.Set(prefRecoveryRecordingMode, "");
            Preferences.Set(prefRecoveryStartTime, "0");
            Preferences.Set(prefRecoveryTimeOfLastUpdate, "0");
            Preferences.Set(prefRecoveryLocationID, "0");
            Preferences.Set(prefRecoveryLocationType, "");
            Preferences.Set(prefRecoveryLocationName, "");
            Preferences.Set(prefRecoveryLocationLat, "0");
            Preferences.Set(prefRecoveryLocationLon, "0");

            Preferences.Set(prefRecoverytransportOriginType, "");
            Preferences.Set(prefRecoverytransportOriginName, "");
            Preferences.Set(prefRecoverytransportOriginID, "0");

            Preferences.Set(prefRecoverytransportLineType, "");
            Preferences.Set(prefRecoverytransportLineName, "");
            Preferences.Set(prefRecoverytransportLineID, "0");
            Preferences.Set(prefCO2MonitorType, "");

            Preferences.Set(prefRecoveryWindows, false);
            Preferences.Set(prefRecoveryVentilation, false);
            Preferences.Set(prefRecoveryCustomNotes, "");

            recordingMode = "";
            startTime = 0;
            timeOfLastUpdate = 0;

            transportLineID = 0;
            transportLineName = "";
            transportLineType = "";

            transportOriginID = 0;
            transportOriginName = "";
            transportOriginType = "";

            locationID = 0;
            locationType = "";
            locationName = "";
            locationLat = 0;
            locationLon = 0;
            sensorValues = new List<int>();
            CO2MonitorType = "";
            ventilation = false;
            windowsOpen = false;
            customNotes = "";
            ReadFromPreferences();
        }
    }

}
