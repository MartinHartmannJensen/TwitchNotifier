using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using IWshRuntimeLibrary;
using Microsoft.Win32;

namespace ArethruTwitchNotifier
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        //Alternative method

        //private void StartupShortcutHandling()
        //{
        //    string shortcutFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\ArethruTwitchNotifierShortcut.lnk";

        //    if (Properties.Settings.Default.StartWithWindows)
        //    {
        //        if (!System.IO.File.Exists(shortcutFilePath))
        //        {
        //            object startupDir = (object)"Startup";
        //            WshShell shell = new WshShell();
        //            string shortcutAdr = (string)shell.SpecialFolders.Item(ref startupDir) + @"\ArethruTwitchNotifierShortcut.lnk";
        //            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAdr);
        //            shortcut.Description = "twitch notification";
        //            shortcut.TargetPath = Environment.CurrentDirectory + @"\ArethruTwitchNotifier.exe";
        //            shortcut.Save();
        //        }
        //    }
        //    else if (System.IO.File.Exists(shortcutFilePath))
        //    {
        //        System.IO.File.Delete(shortcutFilePath);
        //    }
        //}

        void SetRegistryStartup()
        {
            string name = "ArethruTwitchNotifier";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkBox2.Checked)
                rk.SetValue(name, Application.ExecutablePath.ToString());
            else
                rk.DeleteValue(name, false);
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            numericUpDownUpFreq.Value = Properties.Settings.Default.UpdateFrequency;
            numericUpDownPopTime.Value = Properties.Settings.Default.WindowTimeOnScreen;
            checkBox1.Checked = Properties.Settings.Default.RunAutoUpdateAtStart;
            checkBox2.Checked = Properties.Settings.Default.StartWithWindows;
            checkBox3.Checked = Properties.Settings.Default.StartMinimized;
            checkBox4.Checked = Properties.Settings.Default.PlaySound;
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void btnSetUserTok_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UserToken = textBoxUserTok.Text;
            textBoxUserTok.Clear();
            Properties.Settings.Default.Save();
        }

        private void btnSetUpFreq_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UpdateFrequency = (int)numericUpDownUpFreq.Value;
            Properties.Settings.Default.Save();
        }

        private void btnSetPopTime_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowTimeOnScreen = (int)numericUpDownPopTime.Value;
            Properties.Settings.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RunAutoUpdateAtStart = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartWithWindows = checkBox2.Checked;
            Properties.Settings.Default.Save();

            //StartupShortcutHandling();
            SetRegistryStartup();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartMinimized = checkBox3.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlaySound = checkBox4.Checked;
            Properties.Settings.Default.Save();
        }

        private void btn_OpenFile_Click(object sender, EventArgs e)
        {
            System.IO.Stream soundFile = null;

            System.Windows.Forms.OpenFileDialog opf = new System.Windows.Forms.OpenFileDialog();

            opf.InitialDirectory = "c:\\";
            opf.Filter = "sound files (*.wav)|*.wav";
            opf.FilterIndex = 1;
            opf.RestoreDirectory = true;

            if (opf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((soundFile = opf.OpenFile()) != null)
                    {
                        using (var fileStream = new System.IO.FileStream(Application.StartupPath + @"\nSound.wav", System.IO.FileMode.Create, System.IO.FileAccess.Write))
                        {
                            soundFile.CopyTo(fileStream);
                        }
                    }
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private void btn_ManualEdit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\ArethruTwitchNotifier.exe.config");
        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
            RESTcall.OpenBrowserAuthenticate();

            string coolstring = RESTcall.ListenForResponse();
            textBoxUserTok.Text = coolstring;

        }

        private void btn_Deauth_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twitch.tv/settings/connections");
        }

        private void btn_Restart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
