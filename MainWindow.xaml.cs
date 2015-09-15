using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinForms = System.Windows.Forms;
using System.Drawing;
using ArethruTwitchNotifier;
using System.Text.RegularExpressions;

namespace ArethruNotifier
{
    public partial class MainWindow : Window
    {
        WinForms.NotifyIcon trayicon;
        WinForms.MenuItem trayiconSound;

        private static int shortPopupTime = 10;

        public MainWindow()
        {
            InitializeComponent();

            this.Height = 450;
            this.Width = 660;

            SetActivePanel(2);

            #region Initialize Tray Icon
            trayicon = new WinForms.NotifyIcon();
            trayicon.Icon = new System.Drawing.Icon("ATNlogo.ico");
            trayicon.BalloonTipText = "Twitch Notifier";
            trayicon.ContextMenu = new WinForms.ContextMenu(new WinForms.MenuItem[4]
            {
                new WinForms.MenuItem("Refresh", trayicon_RefreshStreamInfo),
                new WinForms.MenuItem("Main Window", trayicon_OpenMainWindow),
                new WinForms.MenuItem("Sound", trayicon_Sound),
                new WinForms.MenuItem("Exit", trayicon_Exit)
            });

            trayiconSound = trayicon.ContextMenu.MenuItems[2];
            trayicon.MouseUp += trayicon_Click;

            #endregion

            #region Apply Config Settings
            if (MHJ_ConfigManager.Settings.I.StartMinimized)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
                this.OnStateChanged(EventArgs.Empty);
            }

            StreamContainer.Instance.FoundNewStreamEvent += StreamContainer_NewStreamFound;

            if (MHJ_ConfigManager.Settings.I.RunAutoUpdateAtStart)
                MyThreading.Instance.StartStreamContainerUpdater();

            #endregion

            #region Settings Startup
            boxUpdFreq.Text = MHJ_ConfigManager.Settings.I.UpdateFrequency.ToString();
            boxPopTime.Text = MHJ_ConfigManager.Settings.I.WindowTimeOnScreen.ToString();
            chkUpd.IsChecked = MHJ_ConfigManager.Settings.I.RunAutoUpdateAtStart;
            chkWin.IsChecked = MHJ_ConfigManager.Settings.I.StartWithWindows;
            chkMin.IsChecked = MHJ_ConfigManager.Settings.I.StartMinimized;
            chkSound.IsChecked = MHJ_ConfigManager.Settings.I.PlaySound;
            trayiconSound.Checked = MHJ_ConfigManager.Settings.I.PlaySound;

            var monitors = WinForms.Screen.AllScreens;
            var dropArr = new int[monitors.Length];
            for (int i = 0; i < monitors.Length; i++)
            {
                dropArr[i] = i;
            }
            dropMonitorSelect.ItemsSource = dropArr;
            dropMonitorSelect.SelectedIndex = MHJ_ConfigManager.Settings.I.DisplayMonitor;
            #endregion
        }

        public void UpdateFollowsList()
        {
            var tup = StreamContainer.Instance.GetFollows();

            if (tup == null)
            {
                DevConsoleOutput.AppendText("Error Getting follows");
                return;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => 
            {
                FollowsList.ItemsSource = tup.Item1;
                FollowsList2.ItemsSource = tup.Item2;
            }));
        }

        private void SetActivePanel(int index)
        {
            switch (index)
            {
                case 1:
                    FollowPanel.Visibility = System.Windows.Visibility.Visible;
                    SettingsPanel.Visibility = System.Windows.Visibility.Hidden;
                    ConsolePanel.Visibility = System.Windows.Visibility.Hidden;
                    btn_Follows.Foreground = (SolidColorBrush)this.FindResource("Color_ActivePurple");
                    btn_Settings.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case 2:
                    FollowPanel.Visibility = System.Windows.Visibility.Hidden;
                    SettingsPanel.Visibility = System.Windows.Visibility.Visible;
                    ConsolePanel.Visibility = System.Windows.Visibility.Hidden;
                    btn_Settings.Foreground = (SolidColorBrush)this.FindResource("Color_ActivePurple");
                    btn_Follows.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case 3:
                    FollowPanel.Visibility = System.Windows.Visibility.Hidden;
                    SettingsPanel.Visibility = System.Windows.Visibility.Hidden;
                    ConsolePanel.Visibility = System.Windows.Visibility.Visible;
                    btn_Settings.Foreground = new SolidColorBrush(Colors.White);
                    btn_Follows.Foreground = new SolidColorBrush(Colors.White);
                    break;
                default:
                    break;
            }
        }

        private void StreamContainer_NewStreamFound(StreamsInfo si)
        {
            MyThreading.Instance.DisplayNotification(si, MHJ_ConfigManager.Settings.I.WindowTimeOnScreen);
            MyThreading.Instance.PlaySound("nSound.wav");
            UpdateFollowsList();
        }

        private void trayicon_RefreshStreamInfo(object obj, EventArgs e)
        {
            StreamContainer.Instance.UpdateInfo();
            MyThreading.Instance.DisplayNotification(StreamContainer.Instance.CurrentInfo, shortPopupTime);
        }

        private void btn_Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(2);
        }

        private void btn_Follows_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetActivePanel(1);
        }

        private void trayicon_OpenMainWindow(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.WindowState.Normal;
            trayicon.Visible = false;
        }

        private void trayicon_Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trayicon_Click(object sender, WinForms.MouseEventArgs e)
        {
            if (e.Button == WinForms.MouseButtons.Left)
            {
                MyThreading.Instance.DisplayNotification(StreamContainer.Instance.CurrentInfo, shortPopupTime);
            }
            else if (e.Button == WinForms.MouseButtons.Middle)
            {
                trayicon_OpenMainWindow(this, EventArgs.Empty);
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (System.Windows.WindowState.Minimized == this.WindowState)
            {
                trayicon.Visible = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void btnAuth_Click(object sender, RoutedEventArgs e)
        {
            RESTcall.OpenBrowserAuthenticate();
            inputUsertoken.Text = RESTcall.ListenForResponse();
        }

        private void btnDeAuth_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twitch.tv/settings/connections");
        }

        private void btnTokSave_Click(object sender, RoutedEventArgs e)
        {
            MHJ_ConfigManager.Settings.I.UserToken = inputUsertoken.Text;
            inputUsertoken.Clear();
        }

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
                        using (var fileStream = new System.IO.FileStream(WinForms.Application.StartupPath + @"\nSound.wav", System.IO.FileMode.Create, System.IO.FileAccess.Write))
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

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            WinForms.Application.Restart();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void btnInstallPath_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory);
        }

        private void btnConfigEdit_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\config.mhjconfig");
        }

        private void chkUpd_Checked(object sender, RoutedEventArgs e)
        {
            MHJ_ConfigManager.Settings.I.RunAutoUpdateAtStart = (bool)chkUpd.IsChecked;
        }

        private void chkWin_Checked(object sender, RoutedEventArgs e)
        {
            MHJ_ConfigManager.Settings.I.StartWithWindows = (bool)chkWin.IsChecked;

            MiscOperation.SetRegistryStartup((bool)chkWin.IsChecked);
        }

        private void chkMin_Checked(object sender, RoutedEventArgs e)
        {
            MHJ_ConfigManager.Settings.I.StartMinimized = (bool)chkMin.IsChecked;
        }

        private void chkSound_Checked(object sender, RoutedEventArgs e)
        {
            MHJ_ConfigManager.Settings.I.PlaySound = (bool)chkSound.IsChecked;
            trayiconSound.Checked = (bool)chkSound.IsChecked;
        }

        private void trayicon_Sound(object sender, EventArgs e)
        {
            if (trayiconSound.Checked)
            {
                trayiconSound.Checked = false;
                chkSound.IsChecked = false;
                MHJ_ConfigManager.Settings.I.PlaySound = false;
            }
            else
            {
                trayiconSound.Checked = true;
                chkSound.IsChecked = true;
                MHJ_ConfigManager.Settings.I.PlaySound = true;
            }
        }

        private void PreviewInputBoxNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
        }

        private void boxUpdFreq_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MHJ_ConfigManager.Settings.I.UpdateFrequency = int.Parse(boxUpdFreq.Text);
                Keyboard.ClearFocus();
            }
        }

        private void boxPopTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MHJ_ConfigManager.Settings.I.WindowTimeOnScreen = int.Parse(boxPopTime.Text);
                Keyboard.ClearFocus();
            }
        }

        private void DevConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                switch (DevConsoleInput.Text.ToUpper())
                {
                    case "HELLO":
                        DevConsoleOutput.AppendText("World");
                        break;
                    case "STREAMSTRING":
                        DevConsoleOutput.AppendText(RESTcall.GetLiveStreamsFullString());
                        break;
                    case "C":
                    case "CLEAR":
                        DevConsoleOutput.Document.Blocks.Clear();
                        break;
                    default:
                        break;
                }
                DevConsoleOutput.AppendText("\n");
                DevConsoleInput.Clear();
            }
        }

        private void dropMonitorSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MHJ_ConfigManager.Settings.I.DisplayMonitor = dropMonitorSelect.SelectedIndex;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                SetActivePanel(3);
            }
        }

        private void Hyperlink_Click(object sender, MouseButtonEventArgs e)
        {
            var obj = (TextBlock)sender;
            System.Diagnostics.Process.Start(obj.ToolTip.ToString());
        }
    }
}
