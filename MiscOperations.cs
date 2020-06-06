using System;
using System.IO;
using WinForms = System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;
using System.Xml.Linq;
using System.Data;

namespace ArethruNotifier {
    public static class MiscOperations {

        public static string RunFileName = "ArethruNotifier.exe";
        public static string StreamFileName = "StreamStart.cmd";

        static string xmldocpath = ConfigMgnr.I.FolderPath + @"\favorites.xml";
        static string xmlfile = "favorites.xml";
        static string xmlFolder = "group sounds";

        public static void RemoveConfig() {
            try {
                Directory.Delete(ConfigMgnr.I.FolderPath, true);
            }
            catch (Exception) {
            }
        }

        public static void CreateStreamLaunchFile(string FolderPath) {
            Directory.CreateDirectory(FolderPath);
            if (!File.Exists(string.Format(@"{0}\{1}", FolderPath, StreamFileName))) {
                using (StreamWriter sw = new StreamWriter(FolderPath + @"\" + StreamFileName)) {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("");
                    sw.WriteLine("ECHO Script not configured. Edit file at");
                    sw.WriteLine("ECHO %~dp0" + StreamFileName);
                    sw.WriteLine("ECHO passed parameter: %1");
                    sw.WriteLine("SET /p stopper=Press To Continue");
                    sw.WriteLine("");
                    sw.WriteLine(":: This is for those who wants to specifically target a certain program to view the stream with");
                }
            }
        }

        public static void SetRegistryStartup(bool SetKey) {
            string name = "ArethruNotifier";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (SetKey)
                rk.SetValue(name, string.Format(@"{0}\{1}", WinForms.Application.StartupPath, RunFileName), RegistryValueKind.String);
            else
                rk.DeleteValue(name, false);

            rk.Close();
        }



        public static void CreateFavoriteConfig(string FolderPath) {
            Directory.CreateDirectory(string.Format(@"{0}\{1}", FolderPath, xmlFolder));
            if (!File.Exists(string.Format(@"{0}\{1}", FolderPath, xmlfile))) {
                using (StreamWriter sw = new StreamWriter(FolderPath + @"\" + xmlfile)) {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sw.WriteLine("<favorites>");
                    sw.WriteLine("  <group name=\"one group of streamers\" priority=\"9001\" soundfile=\"airhorn.wav\" poptime=\"360\">");
                    sw.WriteLine("      <stream>SomeChannelName</stream>");
                    sw.WriteLine("      <stream>SomeOtherChannel</stream>");
                    sw.WriteLine("  </group>");
                    sw.WriteLine("</favorites>");
                    sw.WriteLine("<!--");
                    sw.WriteLine("Name: A name");
                    sw.WriteLine("Priority: The group with the highest number takes priority for playing its sound");
                    sw.WriteLine("Soundfile: These go in 'group sounds' and must be a .wav file");
                    sw.WriteLine("Poptime: How long the popup should stay up for the group");
                    sw.WriteLine("-->");
                }
            }
        }
        /// <summary>
        /// Check if a stream exists within a group in the xml doc. 
        /// </summary>
        /// <param name="name">The stream to search for</param>
        /// <param name="result">This defaults to null on failure</param>
        /// <returns>isSuccess</returns>
        public static bool TryGetFavourite(string name, out FavouriteGroup result) {
            if (!File.Exists(xmldocpath)) {
                CreateFavoriteConfig(ConfigMgnr.I.FolderPath);
            }
            var xdoc = XDocument.Load(xmldocpath);

            var query = from x in xdoc.Root.Elements("group").Elements("stream")
                        where x.Value.Equals(name)
                        select x;

            var bufferresult = query.ToArray();

            if (bufferresult.Length > 0) {
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
                return true;
            }

            result = null;
            return false;
        }
    }

    public class FavouriteGroup {
        string name = "";
        public string Name {
            get { return name; }
            set {
                if (value != null)
                    name = value;
            }
        }

        int priority = -1;
        public int Priority {
            get { return priority; }
            set {
                priority = value;
            }
        }

        string soundfile = "nofile";
        public string Soundfile {
            get { return soundfile; }
            set {
                if (value != null)
                    soundfile = value;
            }
        }

        int poptime = 60;
        public int Poptime {
            get { return poptime; }
            set {
                poptime = value;
            }
        }

        public FavouriteGroup() { }
    }
}
