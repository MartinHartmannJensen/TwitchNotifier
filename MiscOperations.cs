using System;
using System.IO;
using WinForms = System.Windows.Forms;
using Microsoft.Win32;

namespace ArethruNotifier
{
    public static class MiscOperations
    {
        public static string RunFileName = "LaunchArethruNotifier.cmd";
        public static string StreamFileName = "StreamStart.cmd";


        public static void CreateRunFile()
        {
            if (!File.Exists(string.Format(@"{0}\{1}", Environment.CurrentDirectory, RunFileName)))
            {
                using (StreamWriter sw = new StreamWriter(RunFileName))
                {
                    sw.Write(@"START /d " + '\u0022' + Environment.CurrentDirectory + '\u0022' + " " + WinForms.Application.ProductName + ".exe");
                }
            }
        }

        public static void CreateStreamLaunchFile()
        {
            if (!File.Exists(string.Format(@"{0}\{1}", Environment.CurrentDirectory, StreamFileName)))
            {
                using (StreamWriter sw = new StreamWriter(StreamFileName))
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

        public static void SetRegistryStartup(bool checkValue)
        {
            CreateRunFile();

            string name = "ArethruNotifier";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkValue)
                rk.SetValue(name, string.Format(@"{0}\{1}", Environment.CurrentDirectory, RunFileName));
            else
                rk.DeleteValue(name, false);
        }
    }
}
