using System;
using System.IO;
using WinForms = System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;
using System.Xml.Linq;

namespace ArethruNotifier
{
    public static class MiscOperations
    {
        public static string RunFileName = "ArethruNotifier.exe";
        public static string StreamFileName = "StreamStart.cmd";

        static string xmldocpath = ConfigMgnr.I.FolderPath + @"\favourites.xml";



        public static void CreateStreamLaunchFile(string FolderPath)
        {
            Directory.CreateDirectory(FolderPath);
            if (!File.Exists(string.Format(@"{0}\{1}", FolderPath, StreamFileName)))
            {
                using (StreamWriter sw = new StreamWriter(FolderPath + @"\" + StreamFileName))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("");
                    sw.WriteLine("ECHO Script not configured. Edit file at");
                    sw.WriteLine("ECHO %~dp0" + StreamFileName);
                    sw.WriteLine("ECHO passed parameter: %1");
                    sw.WriteLine("SET /p stopper=Press To Continue");
                    sw.WriteLine("");
                    sw.WriteLine(":: Example of how this could look if you have livestreamer installed and added to your path");
                    sw.WriteLine(":: livestreamer twitch.tv/%1 source");
                }
            }
        }

        public static void SetRegistryStartup(bool SetKey)
        {
            string name = "ArethruNotifier";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (SetKey)
                rk.SetValue(name, string.Format(@"{0}\{1}", WinForms.Application.StartupPath, RunFileName), RegistryValueKind.String);
            else
                rk.DeleteValue(name, false);

            rk.Close();
        }

        /// <summary>
        /// Check if a stream exists within a group in the xml doc. 
        /// </summary>
        /// <param name="name">The stream to search for</param>
        /// <param name="result"></param>
        /// <returns>Returns 1 if true, 0 if false.</returns>
        public static int TryGetFavourite(string name, out FavouriteGroup result)
        {
            var xdoc = XDocument.Load(xmldocpath);

            var query = from x in xdoc.Root.Elements("group").Elements("stream")
                        where x.Value.Equals(name)
                        select x;

            var bufferresult = query.ToArray();

            if (bufferresult.Length > 0)
            {
                var xgp = bufferresult[0].Parent;
                var _result = new FavouriteGroup();
                int tempint;
                string tempints;

                _result.Name = xgp.Attribute("name").Value;
                _result.Soundfile = xgp.Attribute("soundfile").Value;
                tempints = xgp.Attribute("priority").Value;
                if (int.TryParse(tempints, out tempint))
                    _result.Priority = tempint;
                tempints = xgp.Attribute("poptime").Value;
                if (int.TryParse(tempints, out tempint))
                    _result.Poptime = tempint;

                result = _result;
                return 1;
            }

            result = null;
            return 0;
        }
    }

    public class FavouriteGroup
    {
        string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                if (value != null)
                    name = value;
            }
        }

        int priority = -1;
        public int Priority {
            get { return priority; }
            set
            {
                priority = value;
            }
        }

        string soundfile = "nofile";
        public string Soundfile
        {
            get { return soundfile; }
            set
            {
                if (value != null)
                    soundfile = value;
            }
        }

        int poptime = 60;
        public int Poptime
        {
            get { return poptime; }
            set
            {
                poptime = value;
            }
        }

        public FavouriteGroup() { }
    }
}
