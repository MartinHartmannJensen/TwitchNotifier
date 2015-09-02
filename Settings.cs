using System;
using System.IO;

namespace MHJ_ConfigManager
{
    public class Settings
    {
        public static string configPath = Environment.CurrentDirectory + @"\config.mhjconfig";

        public static string defaultConfigPath = @"default.mhjconfig";

        #region Instantiating
        static MHJ_ConfigManager.Settings instance = null;

        public static MHJ_ConfigManager.Settings I { get { if (instance == null) instance = new MHJ_ConfigManager.Settings(); return instance; } }

        private Settings()
        {
            if (!File.Exists(configPath))
                CreateDefaultConfigFile();
        }
        #endregion

        #region ReadAndWrite
        public static string ReadDocValue(string key)
        {
            foreach (string line in File.ReadLines(configPath))
            {
                if (line.StartsWith(key))
                {
                    var sstr = line.Substring(key.Length + 1);

                    return sstr;
                }
            }

            return "nothing found";
        }

        public static void WriteDocValue(string key, string value)
        {
            string[] allLines = File.ReadAllLines(configPath);

            using (StreamWriter sr = new StreamWriter(configPath))
            {
                foreach (var item in allLines)
                {
                    if (item.StartsWith(key))
                        sr.WriteLine(key + "=" + value);
                    else
                        sr.WriteLine(item);
                }
            }

        }

        public static void CreateDefaultConfigFile()
        {
            using (StreamWriter sw = new StreamWriter(configPath))
            {
                sw.WriteLine("UserToken=nouser");
                sw.WriteLine("UpdateFrequency=42");
                sw.WriteLine("WindowTimeOnScreen=1200");
                sw.WriteLine("RunAutoUpdateAtStart=True");
                sw.WriteLine("StartWithWindows=False");
                sw.WriteLine("StartMinimized=False");
                sw.WriteLine("PlaySound=False");
            }
        }
        #endregion

        #region Properties
        public string UserToken { get { return ReadDocValue("UserToken"); } set { WriteDocValue("UserToken", (string)value); } }
        public int UpdateFrequency { get { return int.Parse(ReadDocValue("UpdateFrequency")); } set { WriteDocValue("UpdateFrequency", value.ToString()); } }
        public int WindowTimeOnScreen { get { return int.Parse(ReadDocValue("WindowTimeOnScreen")); } set { WriteDocValue("WindowTimeOnScreen", value.ToString()); } }
        public bool RunAutoUpdateAtStart { get { return Boolean.Parse(ReadDocValue("RunAutoUpdateAtStart")); } set { WriteDocValue("RunAutoUpdateAtStart", value.ToString()); } }
        public bool StartWithWindows { get { return Boolean.Parse(ReadDocValue("StartWithWindows")); } set { WriteDocValue("StartWithWindows", value.ToString()); } }
        public bool StartMinimized { get { return Boolean.Parse(ReadDocValue("StartMinimized")); } set { WriteDocValue("StartMinimized", value.ToString()); } }
        public bool PlaySound { get { return Boolean.Parse(ReadDocValue("PlaySound")); } set { WriteDocValue("PlaySound", value.ToString()); } }
		
		
        #endregion
    }
}
