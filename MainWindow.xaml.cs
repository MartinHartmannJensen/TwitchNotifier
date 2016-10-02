﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WinForms = System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ArethruNotifier
{

    public partial class MainWindow : Window
    {
        AN_Console Acon;
        WinForms.NotifyIcon trayicon;
        WinForms.MenuItem trayiconSound;
        WinForms.MenuItem trayiconMonitor;

        public MainWindow()
        {
            InitializeComponent();
            Acon = new AN_Console(DevConsoleOutput, this);

            this.KeyDown += Window_KeyDown;
            this.Closing += new System.ComponentModel.CancelEventHandler((object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                ConfigMgnr.I.Save();
                System.Threading.Thread.Sleep(100);
            });

            InitializeTrayIcon();
            Set_SettingsUI();
            DoStartupFunctions();


            if (!ConfigMgnr.I.Color_MainPanel.Equals("0"))
                Resources["Color_MainPanel"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_MainPanel));
            if (!ConfigMgnr.I.Color_SubPanel.Equals("0"))
                Resources["Color_SubPanel"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_SubPanel));
            if (!ConfigMgnr.I.Color_DefaultText.Equals("0"))
                Resources["Color_DefaultText"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_DefaultText));
            if (!ConfigMgnr.I.Color_Highlight.Equals("0"))
                Resources["Color_Highlight"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_Highlight));
            if (!ConfigMgnr.I.Color_BtnBG.Equals("0"))
                Resources["Color_BtnBG"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_BtnBG));
        }
        
        #region Misc. Methods

        

        void DoStartupFunctions()
        {
            MiscOperations.CreateStreamLaunchFile(ConfigMgnr.I.FolderPath);
            MiscOperations.CreateFavoriteConfig(ConfigMgnr.I.FolderPath);
            MiscOperations.SetRegistryStartup(ConfigMgnr.I.StartWithWindows);

            if (!ConfigMgnr.I.OfflineMode)
            {
                NotifyCtr.Instance.StartStreaminfoUpdater();
            }
            if (ConfigMgnr.I.StartMinimized)
            {
                this.WindowState = WindowState.Minimized;
                this.OnStateChanged(EventArgs.Empty);
            }
        }

        void InitializeTrayIcon()
        {
            trayicon = new WinForms.NotifyIcon();
            trayicon.Icon = Properties.Resources.ANlogo24bit;
            trayicon.BalloonTipText = "Arethru Twitch Notifier";
            trayicon.ContextMenu = new WinForms.ContextMenu(new WinForms.MenuItem[]
            {
                new WinForms.MenuItem("Monitor"),
                new WinForms.MenuItem("Refresh", trayicon_RefreshStreamInfo),
                new WinForms.MenuItem("Main Window", trayicon_OpenMainWindow),
                new WinForms.MenuItem("Sound", trayicon_Sound),
                new WinForms.MenuItem("Exit", trayicon_Exit)
            });

            trayiconSound = trayicon.ContextMenu.MenuItems[3];
            trayiconMonitor = trayicon.ContextMenu.MenuItems[0];
            trayicon.MouseUp += trayicon_Click;

            trayicon.Visible = true;
        }

        void Set_SettingsUI()
        {
            boxUpdFreq.Text = ConfigMgnr.I.UpdateFrequency.ToString();
            boxPopTime.Text = ConfigMgnr.I.NotificationScreenTime.ToString();
            chkUpd.IsChecked = ConfigMgnr.I.OfflineMode;
            chkWin.IsChecked = ConfigMgnr.I.StartWithWindows;
            chkMin.IsChecked = ConfigMgnr.I.StartMinimized;
            chkSound.IsChecked = ConfigMgnr.I.PlaySound;
            trayiconSound.Checked = ConfigMgnr.I.PlaySound;
            chkScript.IsChecked = ConfigMgnr.I.OpenStreamWithScript;


            //Monitor selection setup
            var monitors = WinForms.Screen.AllScreens;
            var dropArr = new int[monitors.Length];
            for (int i = 0; i < monitors.Length; i++)
            {
                dropArr[i] = i;
                trayiconMonitor.MenuItems.Add(new WinForms.MenuItem(i.ToString(), trayicon_MonitorItem));
            }
            dropMonitorSelect.ItemsSource = dropArr;
            dropMonitorSelect.SelectedIndex = ConfigMgnr.I.DisplayMonitor;
            trayiconMonitor.MenuItems[ConfigMgnr.I.DisplayMonitor].Checked = true;
        }

        public void UpdateFollowsList()
        {
            var originalTitle = this.Title;
            this.Title = originalTitle + " - Fetching list, hold on!";
            System.Threading.Thread t = new System.Threading.Thread(() => {
                var tup = TwitchDataHandler.Instance.GetFollows();
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if (tup != null)
                    {
                        FollowsList.ItemsSource = tup.Item1;
                        FollowsList2.ItemsSource = tup.Item2;
                    }
                    this.Title = originalTitle;
                }));
            });
            t.Start();
        }

        void SetActivePanel(int i)
        {
            //TODO better solution
            Panel[] pans = { FollowPanel, SettingsPanel, ConsolePanel };

            foreach (var item in pans)
            {
                item.Visibility = Visibility.Hidden;
            }
            pans[i].Visibility = Visibility.Visible;
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
        }

        void SetTrayMonitorSelection(int selection)
        {
            foreach (WinForms.MenuItem item in trayiconMonitor.MenuItems)
            {
                item.Checked = false;
            }
            trayiconMonitor.MenuItems[selection].Checked = true;
        }

        #endregion


        #region Trayicon Eventhandlers

        private void trayicon_RefreshStreamInfo(object obj, EventArgs e)
        {
            TwitchDataHandler.Instance.UpdateInfo();
            NotifyCtr.Instance.DisplayNotification(TwitchDataHandler.Instance.CurrentInfo, ConfigMgnr.I.NotificationScreenTime);
            NotifyCtr.Instance.StopSound();
        }

        private void trayicon_OpenMainWindow(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.WindowState.Normal;
            //trayicon.Visible = false;
        }

        private void trayicon_Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trayicon_Click(object sender, WinForms.MouseEventArgs e)
        {
            if (e.Button == WinForms.MouseButtons.Left)
            {
                NotifyCtr.Instance.DisplayNotification(TwitchDataHandler.Instance.CurrentInfo, ConfigMgnr.I.NotificationScreenTime);
            }
            else if (e.Button == WinForms.MouseButtons.Middle)
            {
                trayicon_OpenMainWindow(this, EventArgs.Empty);
            }
        }

        private void trayicon_Sound(object sender, EventArgs e)
        {
            if (trayiconSound.Checked)
            {
                trayiconSound.Checked = false;
                chkSound.IsChecked = false;
                ConfigMgnr.I.PlaySound = false;
            }
            else
            {
                trayiconSound.Checked = true;
                chkSound.IsChecked = true;
                ConfigMgnr.I.PlaySound = true;
            }
        }

        private void trayicon_MonitorItem(object sender, EventArgs e)
        {
            var m = (WinForms.MenuItem)sender;
            int selection = int.Parse(m.Text);
            SetTrayMonitorSelection(selection);
            dropMonitorSelect.SelectedIndex = selection;
            ConfigMgnr.I.DisplayMonitor = selection;
        }


        #endregion


        #region Eventhandlers

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (System.Windows.WindowState.Minimized == this.WindowState)
            {
                //trayicon.Visible = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void btnAuth_Click(object sender, RoutedEventArgs e)
        {
            WebComm.OpenBrowserAuthenticate();
            string user_tok = WebComm.ListenForResponse();
            DevConsoleOutput.AppendText("Usertoken Saved: " + user_tok + "\n");
            ConfigMgnr.I.UserToken = user_tok;
        }

        private void btnDeAuth_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twitch.tv/settings/connections");
        }

        //private void btnTokSave_Click(object sender, RoutedEventArgs e)
        //{
        //    UserSettings.Default.UserToken = inputUsertoken.Text;
        //    inputUsertoken.Clear();
        //}

        private void btnSoundSelect_Click(object sender, RoutedEventArgs e)
        {
            System.IO.Stream soundFile = null;

            WinForms.OpenFileDialog opf = new WinForms.OpenFileDialog();

            opf.InitialDirectory = "c:\\";
            opf.Filter = "sound files (*.wav)|*.wav";
            opf.FilterIndex = 1;
            opf.RestoreDirectory = true;

            if (opf.ShowDialog() == WinForms.DialogResult.OK)
            {
                try
                {
                    if ((soundFile = opf.OpenFile()) != null)
                    {
                        using (var fileStream = new System.IO.FileStream(ConfigMgnr.I.FolderPath + @"\sound.wav", System.IO.FileMode.Create, System.IO.FileAccess.Write))
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

        private void DevConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Acon.In = DevConsoleInput.Text;
                DevConsoleInput.Clear();
            }
        }

        private void Navbar_Click(object sender, MouseButtonEventArgs e)
        {
            var s = (TextBlock)sender;
            var defCol = (SolidColorBrush)FindResource("Color_DefaultText");
            var actCol = (SolidColorBrush)FindResource("Color_Highlight");
            TextBlock[] navBtns = { btn_Follows, btn_Settings };

            for (int i = 0; i < navBtns.Length; i++)
            {
                if (navBtns[i].Equals(s))
                {
                    SetActivePanel(i);
                    navBtns[i].Foreground = actCol;
                }
                else
                    navBtns[i].Foreground = defCol;
            }

            if (s.Equals(btn_Follows))
                UpdateFollowsList();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            ConfigMgnr.I.Save();
            System.Threading.Thread.Sleep(100);
            WinForms.Application.Restart();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void btnConfigPath_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ConfigMgnr.I.FolderPath);
        }

        //private void btnConfigEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\config.mhjconfig");
        //}

        private void chkUpd_Checked(object sender, RoutedEventArgs e)
        {
            ConfigMgnr.I.OfflineMode = (bool)chkUpd.IsChecked;
        }

        private void chkWin_Checked(object sender, RoutedEventArgs e)
        {
            ConfigMgnr.I.StartWithWindows = (bool)chkWin.IsChecked;

            MiscOperations.SetRegistryStartup((bool)chkWin.IsChecked);
        }

        private void chkMin_Checked(object sender, RoutedEventArgs e)
        {
            ConfigMgnr.I.StartMinimized = (bool)chkMin.IsChecked;
        }

        private void chkSound_Checked(object sender, RoutedEventArgs e)
        {
            ConfigMgnr.I.PlaySound = (bool)chkSound.IsChecked;
            trayiconSound.Checked = (bool)chkSound.IsChecked;
        }

        private void PreviewInputBoxNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void boxUpdFreq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //ConfigMgnr.I.UpdateFrequency = int.Parse(boxUpdFreq.Text);
                Keyboard.ClearFocus();
            }
        }

        private void boxPopTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //ConfigMgnr.I.NotificationScreenTime = int.Parse(boxPopTime.Text);
                Keyboard.ClearFocus();
            }
        }

        private void dropMonitorSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetTrayMonitorSelection(dropMonitorSelect.SelectedIndex);
            ConfigMgnr.I.DisplayMonitor = dropMonitorSelect.SelectedIndex;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                SetActivePanel(2);
                DevConsoleInput.Focus();
            }
        }

        private void Hyperlink_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = (TextBlock)sender;
            System.Diagnostics.Process.Start(obj.ToolTip.ToString());
        }

        private void sourceCodeLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MartinHartmannJensen/TwitchNotifier#arethrunotifier");
        }

        private void chkScript_Click(object sender, RoutedEventArgs e)
        {
            ConfigMgnr.I.OpenStreamWithScript = (bool)chkScript.IsChecked;
        }

        private void boxUpdFreq_TextChanged(object sender, TextChangedEventArgs e)
        {
            var s = (TextBox)sender;
            int numba;
            int.TryParse(s.Text, out numba);
            if (numba < 10)
                numba = 10;
            ConfigMgnr.I.UpdateFrequency = numba;
        }

        private void boxPopTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            var s = (TextBox)sender;
            int numba;
            int.TryParse(s.Text, out numba);
            if (numba < 3)
                numba = 3;
            ConfigMgnr.I.NotificationScreenTime = numba;
        }

        #endregion
    }
}
