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
        /// <param name="result">(System.Xml.Linq.XElement) The stream element with the tree info attached</param>
        /// <returns>Returns 1 if true, 0 if false.</returns>
        public static int TryGetFavourite(string name, out XElement result)
        {
            var xdoc = XDocument.Load(xmldocpath);

            var query = from x in xdoc.Root.Elements("group").Elements("stream")
                        where x.Value.Equals(name)
                        select x;

            var bufferresult = query.ToArray();

            if (bufferresult.Length > 0)
            {
                result = bufferresult[0];
                return 1;
            }

            result = null;
            return 0;
        }
    }
}
