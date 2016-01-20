using System;
using System.IO;
using WinForms = System.Windows.Forms;
using Microsoft.Win32;

namespace ArethruNotifier
{
    public static class MiscOperations
    {
        public static void CreateBatRunFile()
        {
            if (!File.Exists(Environment.CurrentDirectory + @"\RunApplication.bat"))
            {
                using (StreamWriter sw = new StreamWriter("RunApplication.bat"))
                {
                    sw.Write(@"START /d " + '\u0022' + Environment.CurrentDirectory + '\u0022' + " " + WinForms.Application.ProductName + ".exe");
                }
            }
        }

        public static void SetRegistryStartup(bool checkValue)
        {
            CreateBatRunFile();

            string name = "ArethruTwitchNotifier";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkValue)
                rk.SetValue(name, Environment.CurrentDirectory + @"\RunApplication.bat");
            else
                rk.DeleteValue(name, false);
        }
    }
}
