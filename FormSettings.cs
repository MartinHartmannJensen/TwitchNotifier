using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace ArethruTwitchNotifier
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            numericUpDownUpFreq.Value = Settings.Default.UpdateFrequency;
            numericUpDownPopTime.Value = Settings.Default.WindowTimeOnScreen;
            checkBox1.Checked = Settings.Default.RunAutoUpdateAtStart;
            checkBox2.Checked = Settings.Default.StartWithWindows;
            checkBox3.Checked = Settings.Default.StartMinimized;
            checkBox4.Checked = Settings.Default.PlaySound;
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();


            string shortcutFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\ArethruTwitchNotifierShortcut.lnk";

            if (Settings.Default.StartWithWindows)
            {
                if (!System.IO.File.Exists(shortcutFilePath))
                {
                    object startupDir = (object)"Startup";
                    WshShell shell = new WshShell();
                    string shortcutAdr = (string)shell.SpecialFolders.Item(ref startupDir) + @"\ArethruTwitchNotifierShortcut.lnk";
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAdr);
                    shortcut.Description = "twitch notification";
                    shortcut.TargetPath = Environment.CurrentDirectory + @"\ArethruTwitchNotifier.exe";
                    shortcut.Save(); 
                }
            }
            else if (System.IO.File.Exists(shortcutFilePath))
            {
                System.IO.File.Delete(shortcutFilePath);
            }
        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
            RESTcall.OpenBrowserAuthenticate();
        }

        private void btnSetUserTok_Click(object sender, EventArgs e)
        {
            Settings.Default.UserToken = textBoxUserTok.Text;
            textBoxUserTok.Clear();
        }

        private void btnSetUpFreq_Click(object sender, EventArgs e)
        {
            Settings.Default.UpdateFrequency = (int)numericUpDownUpFreq.Value;
        }

        private void btnSetPopTime_Click(object sender, EventArgs e)
        {
            Settings.Default.WindowTimeOnScreen = (int)numericUpDownPopTime.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.RunAutoUpdateAtStart = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.StartWithWindows = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.StartMinimized = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.PlaySound = checkBox4.Checked;
        }
    }
}
