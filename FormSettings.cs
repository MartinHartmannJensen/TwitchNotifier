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
using System.Configuration;

namespace ArethruTwitchNotifier
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        void SetRegistryStartup()
        {
            MiscOperation.CreateBatRunFile();

            string name = "ArethruTwitchNotifier";

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkBox2.Checked)
                rk.SetValue(name, Environment.CurrentDirectory + @"\RunApplication.bat");
            else
                rk.DeleteValue(name, false);
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            //numericUpDownUpFreq.Value = Properties.Settings.Default.UpdateFrequency;
            //numericUpDownPopTime.Value = Properties.Settings.Default.WindowTimeOnScreen;
            //checkBox1.Checked = Properties.Settings.Default.RunAutoUpdateAtStart;
            //checkBox2.Checked = Properties.Settings.Default.StartWithWindows;
            //checkBox3.Checked = Properties.Settings.Default.StartMinimized;
            //checkBox4.Checked = Properties.Settings.Default.PlaySound;

            numericUpDownUpFreq.Value = MHJ_ConfigManager.Settings.I.UpdateFrequency;
            numericUpDownPopTime.Value = MHJ_ConfigManager.Settings.I.WindowTimeOnScreen;
            checkBox1.Checked = MHJ_ConfigManager.Settings.I.RunAutoUpdateAtStart;
            checkBox2.Checked = MHJ_ConfigManager.Settings.I.StartWithWindows;
            checkBox3.Checked = MHJ_ConfigManager.Settings.I.StartMinimized;
            checkBox4.Checked = MHJ_ConfigManager.Settings.I.PlaySound;
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void btnSetUserTok_Click(object sender, EventArgs e)
        {
            //Properties.Settings.Default.UserToken = textBoxUserTok.Text;
            //textBoxUserTok.Clear();
            //Properties.Settings.Default.Save();
            MHJ_ConfigManager.Settings.I.UserToken = textBoxUserTok.Text;
            textBoxUserTok.Clear();
        }

        private void btnSetUpFreq_Click(object sender, EventArgs e)
        {
            //Properties.Settings.Default.UpdateFrequency = (int)numericUpDownUpFreq.Value;
            //Properties.Settings.Default.Save();
            MHJ_ConfigManager.Settings.I.UpdateFrequency = (int)numericUpDownUpFreq.Value;
        }

        private void btnSetPopTime_Click(object sender, EventArgs e)
        {
            //Properties.Settings.Default.WindowTimeOnScreen = (int)numericUpDownPopTime.Value;
            //Properties.Settings.Default.Save();
            MHJ_ConfigManager.Settings.I.WindowTimeOnScreen = (int)numericUpDownPopTime.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.RunAutoUpdateAtStart = checkBox1.Checked;
            //Properties.Settings.Default.Save();
            MHJ_ConfigManager.Settings.I.RunAutoUpdateAtStart = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.StartWithWindows = checkBox2.Checked;
            //Properties.Settings.Default.Save();

            MHJ_ConfigManager.Settings.I.StartWithWindows = checkBox2.Checked;

            SetRegistryStartup();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.StartMinimized = checkBox3.Checked;
            //Properties.Settings.Default.Save();
            MHJ_ConfigManager.Settings.I.StartMinimized = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.PlaySound = checkBox4.Checked;
            //Properties.Settings.Default.Save();
            MHJ_ConfigManager.Settings.I.PlaySound = checkBox4.Checked;
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

        private void btn_AppLocation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory);
        }

        private void btn_UserLocation_Click(object sender, EventArgs e)
        {
            //string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            //var trim = @"\user.exe.config";
            //string folder = path.TrimEnd(trim.ToCharArray());
            //System.Diagnostics.Process.Start(folder);

            System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\config.mhjconfig");
        }
    }
}
