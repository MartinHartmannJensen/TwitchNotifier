using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace ArethruNotifier
{
    class ConfigMgnr : CustomConfigProject.Base.SettingsManager
    {
        private static ConfigMgnr instance = new ConfigMgnr();
        public static ConfigMgnr I { get { return instance; } }
        public override string ConfigPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ArethruNotifier\config.ini"; } }

        private ConfigMgnr() : base()
        {
            DefaultCollection["UserToken"] = "notoken";
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
            RunStartup();
        }
        public string UserToken { get { return Get("UserToken"); } set { Set("UserToken", value); } }
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
    }

  
}

namespace CustomConfigProject.Base
{
    /// <summary>
    /// Class containing basic functions for a configuration system, using .ini text files.
    /// Made to be inherited by a class containing properties that reference,
    /// Get and Set.
    /// Important todo:
    /// defaultCollection values,
    /// RunStartup() method in contructor
    /// </summary>
    abstract class SettingsManager
    {
        public abstract string ConfigPath { get; }
        public string FolderPath;

        protected Dictionary<string, string> PropertiesCollection = new Dictionary<string, string>();
        protected Dictionary<string, string> DefaultCollection = new Dictionary<string, string>();

        protected void RunStartup()
        {
            string[] folders = Regex.Split(ConfigPath, @"\\");
            FolderPath = folders[0];
            for (int i = 1; i < folders.Length - 1; i++)
            {
                FolderPath += @"\" + folders[i];
            }

            if (File.Exists(ConfigPath))
            {
                ReadConfiguration();
            }
            else
            {
                SetValuesFromDefault();
                Save();
            }
        }

        protected string Get(string prop)
        {
            if (PropertiesCollection.ContainsKey(prop))
                return PropertiesCollection[prop];
            return DefaultCollection[prop];
        }

        protected void Set(string prop, object value)
        {
            var val = value.ToString();
            PropertiesCollection[prop] = val;
        }

        // Write to config file
        public void Save()
        {
            Directory.CreateDirectory(FolderPath);
            using (StreamWriter sr = new StreamWriter(ConfigPath))
            {
                foreach (var item in PropertiesCollection)
                {
                    sr.WriteLine(item.Key + "=" + item.Value);
                }
            }
        }

        // Read config file and set values
        private void ReadConfiguration()
        {
            foreach (string item in File.ReadLines(ConfigPath))
            {
                string[] key_value = Regex.Split(item, @"\s+=\s+|=\s+|\s+=|=");
                if (DefaultCollection.ContainsKey(key_value[0]))
                    PropertiesCollection[key_value[0]] = key_value[1];
            }
        }

        // In case of no file, set all from default
        private void SetValuesFromDefault()
        {
            foreach (var item in DefaultCollection)
            {
                PropertiesCollection[item.Key] = item.Value;
            }
        }
    }
}