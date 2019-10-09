using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace ArethruNotifier {
    class ConfigMgnr : CustomConfigProject.Base.SettingsManager {
        private static ConfigMgnr instance = new ConfigMgnr();
        public static ConfigMgnr I { get { return instance; } }
        public override string ConfigPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ArethruNotifier\config.ini"; } }

        private ConfigMgnr() : base() {
            DefaultCollection["UserToken"] = "notoken";
            DefaultCollection["UserName"] = "0";
            DefaultCollection["UpdateFrequency"] = "60";
            DefaultCollection["NotificationScreenTime"] = "60";
            DefaultCollection["OfflineMode"] = "False";
            DefaultCollection["StartWithWindows"] = "False";
            DefaultCollection["StartMinimized"] = "False";
            DefaultCollection["PlaySound"] = "True";
            DefaultCollection["DisplayMonitor"] = "0";
            DefaultCollection["OpenStreamWithScript"] = "False";
            DefaultCollection["Color_MainPanel"] = "0";
            DefaultCollection["Color_SubPanel"] = "0";
            DefaultCollection["Color_DefaultText"] = "0";
            DefaultCollection["Color_Highlight"] = "0";
            DefaultCollection["Color_BtnBG"] = "0";
            DefaultCollection["StreamLaunch"] = "0";
            RunStartup();
        }
        public string UserToken { get { return Get("UserToken"); } set { Set("UserToken", value); } }
        public string UserName { get { return Get("UserName"); } set { Set("UserName", value); } }
        public int UpdateFrequency { get { return int.Parse(Get("UpdateFrequency")); } set { Set("UpdateFrequency", value); } }
        public int NotificationScreenTime { get { return int.Parse(Get("NotificationScreenTime")); } set { Set("NotificationScreenTime", value); } }
        public bool OfflineMode { get { return bool.Parse(Get("OfflineMode")); } set { Set("OfflineMode", value); } }
        public bool StartWithWindows { get { return bool.Parse(Get("StartWithWindows")); } set { Set("StartWithWindows", value); } }
        public bool StartMinimized { get { return bool.Parse(Get("StartMinimized")); } set { Set("StartMinimized", value); } }
        public bool PlaySound { get { return bool.Parse(Get("PlaySound")); } set { Set("PlaySound", value); } }
        public int DisplayMonitor { get { return int.Parse(Get("DisplayMonitor")); } set { Set("DisplayMonitor", value); } }
        public bool OpenStreamWithScript { get { return bool.Parse(Get("OpenStreamWithScript")); } set { Set("OpenStreamWithScript", value); } }
        public string Color_MainPanel { get { return Get("Color_MainPanel"); } set { Set("Color_MainPanel", value); } }
        public string Color_SubPanel { get { return Get("Color_SubPanel"); } set { Set("Color_SubPanel", value); } }
        public string Color_DefaultText { get { return Get("Color_DefaultText"); } set { Set("Color_DefaultText", value); } }
        public string Color_Highlight { get { return Get("Color_Highlight"); } set { Set("Color_Highlight", value); } }
        public string Color_BtnBG { get { return Get("Color_BtnBG"); } set { Set("Color_BtnBG", value); } }
        public string StreamLaunch { get { return Get("StreamLaunch"); } set { Set("StreamLaunch", value); } }

        // Special additions
        private NotifyCtr _notify = null;
        public NotifyCtr NotifyController {
            get {
                if (_notify == null) {
                    _notify = new NotifyCtr();
                }
                return _notify;
            }
        }

        private TwitchDataHandler _datahandler = null;
        public TwitchDataHandler DataHandler {
            get {
                if (_datahandler == null) {
                    _datahandler = new TwitchDataHandler();
                }
                return _datahandler;
            }
        }
    }


}

namespace CustomConfigProject.Base {
    /// <summary>
    /// Class containing basic functions for a configuration system, using .ini text files.
    /// Made to be inherited by a class containing properties that reference,
    /// Get and Set.
    /// Important todo:
    /// defaultCollection values,
    /// RunStartup() method in contructor
    /// </summary>
    abstract class SettingsManager {
        public abstract string ConfigPath { get; }
        public string FolderPath { get; protected set; }

        protected Dictionary<string, string> PropertiesCollection = new Dictionary<string, string>();
        protected Dictionary<string, string> DefaultCollection = new Dictionary<string, string>();

        protected void RunStartup() {
            string[] folders = Regex.Split(ConfigPath, @"\\");
            FolderPath = folders[0];
            for (int i = 1; i < folders.Length - 1; i++) {
                FolderPath += @"\" + folders[i];
            }

            if (File.Exists(ConfigPath)) {
                ReadConfiguration();
            }
            else {
                SetValuesFromDefault();
                Save();
            }
        }

        protected string Get(string prop) {
            if (PropertiesCollection.ContainsKey(prop))
                return PropertiesCollection[prop];
            PropertiesCollection[prop] = DefaultCollection[prop];
            return DefaultCollection[prop];
        }

        protected void Set(string prop, object value) {
            var val = value.ToString();
            PropertiesCollection[prop] = val;
        }

        // Write to config file
        public void Save() {
            //List of lines to write to config
            List<string> savetheselines = new List<string>();
            if (File.Exists(ConfigPath)) {
                Dictionary<string, string> PropertiesCollectionCopy = new Dictionary<string, string>();
                //Create this List ^
                foreach (var item in PropertiesCollection) {
                    PropertiesCollectionCopy.Add(item.Key, item.Value);
                }

                //add lines starting with ;, add existing key-value pairs to their position, and remove them from the dictonary
                foreach (var item in File.ReadLines(ConfigPath)) {
                    if (item.StartsWith(";")) {
                        savetheselines.Add(item);
                        continue;
                    }

                    string[] key_value = Regex.Split(item, @"\s+=\s+|=\s+|\s+=|=");

                    if (PropertiesCollectionCopy.ContainsKey(key_value[0])) {
                        savetheselines.Add(key_value[0] + "=" + PropertiesCollectionCopy[key_value[0]]);
                        PropertiesCollectionCopy.Remove(key_value[0]);
                    }
                }

                //add any missing key-value pairs to savetheselines
                foreach (var item in PropertiesCollectionCopy) {
                    savetheselines.Add(item.Key + "=" + item.Value);
                }
            }
            
            Directory.CreateDirectory(FolderPath);
            using (StreamWriter sr = new StreamWriter(ConfigPath)) {
                foreach (var item in savetheselines) {
                    sr.WriteLine(item);
                }
            }
        }

        // Read config file and set values
        private void ReadConfiguration() {
            foreach (string item in File.ReadLines(ConfigPath)) {
                if (item.StartsWith(";"))
                    continue;

                string[] key_value = Regex.Split(item, @"\s+=\s+|=\s+|\s+=|=");
                if (DefaultCollection.ContainsKey(key_value[0]))
                    PropertiesCollection[key_value[0]] = key_value[1];
            }
        }

        // In case of no file, set all from default
        private void SetValuesFromDefault() {
            foreach (var item in DefaultCollection) {
                PropertiesCollection[item.Key] = item.Value;
            }
        }
    }
}