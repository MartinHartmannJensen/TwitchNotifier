using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WinForms = System.Windows.Forms;
using System.Text.RegularExpressions;
using ArethruNotifier.Helix;

namespace ArethruNotifier {

    public partial class MainWindow : Window {
        AN_Console Acon;
        WinForms.NotifyIcon trayicon;
        WinForms.MenuItem trayiconMode;
        WinForms.MenuItem trayiconMonitor;

        public MainWindow() {
            InitializeComponent();
            Acon = new AN_Console(DevConsoleOutput, this);
            this.KeyDown += Window_KeyDown;

            InitializeTrayIcon();
            Set_SettingsUI();
            if (ConfigMgnr.I.StartMinimized) {
                this.WindowState = WindowState.Minimized;
                this.OnStateChanged(EventArgs.Empty);
            }
        }

        #region Misc. Methods

        void InitializeTrayIcon() {
            trayicon = new WinForms.NotifyIcon();
            trayicon.Icon = Properties.Resources.ANlogo24bit;
            trayicon.BalloonTipText = "Arethru Twitch Notifier";
            trayicon.ContextMenu = new WinForms.ContextMenu(new WinForms.MenuItem[]
            {
                new WinForms.MenuItem("Modes"),
                new WinForms.MenuItem("Monitor"),
                new WinForms.MenuItem("Refresh", trayicon_RefreshStreamInfo),
                new WinForms.MenuItem("Main Window", trayicon_OpenMainWindow),
                new WinForms.MenuItem("Exit", trayicon_Exit)
            });

            trayiconMode = trayicon.ContextMenu.MenuItems[0];
            trayiconMonitor = trayicon.ContextMenu.MenuItems[1];
            trayicon.MouseUp += trayicon_Click;

            trayicon.Visible = true;
        }

        void Set_SettingsUI() {
            boxUpdFreq.Text = ConfigMgnr.I.UpdateFrequency.ToString();
            boxPopTime.Text = ConfigMgnr.I.NotificationScreenTime.ToString();
            chkWin.IsChecked = ConfigMgnr.I.StartWithWindows;
            chkMin.IsChecked = ConfigMgnr.I.StartMinimized;
            chkScript.IsChecked = ConfigMgnr.I.OpenStreamWithScript;


            //Monitor selection setup
            var monitors = WinForms.Screen.AllScreens;
            var dropArr = new int[monitors.Length];
            for (int i = 0; i < monitors.Length; i++) {
                dropArr[i] = i;
                trayiconMonitor.MenuItems.Add(new WinForms.MenuItem(i.ToString(), trayicon_MonitorItem));
            }
            dropMonitorSelect.ItemsSource = dropArr;
            dropMonitorSelect.SelectedIndex = ConfigMgnr.I.DisplayMonitor;
            trayiconMonitor.MenuItems[ConfigMgnr.I.DisplayMonitor].Checked = true;

            //Mode selection setup
            var modeNames = Enum.GetNames(typeof(NotifyCtr.NotifyModes));
            foreach (var item in modeNames) {
                trayiconMode.MenuItems.Add(new WinForms.MenuItem(item, trayicon_OnModeClick));
            }
            dropModeSelect.ItemsSource = modeNames;
            dropModeSelect.SelectedIndex = ConfigMgnr.I.Mode;
            trayiconMode.MenuItems[ConfigMgnr.I.Mode].Checked = true;
        }

        public async void UpdateFollowsList() {
            var fl = await ConfigMgnr.I.NotifyController.DataHandler.GetFollowLists();
            if (fl.isGood) {
                FollowsList.ItemsSource = fl.online;
                FollowsList2.ItemsSource = fl.offline;
            }
        }

        void SetActivePanel(int i) {
            Panel[] pans = { FollowPanel, SettingsPanel, ConsolePanel, UninstallPanel };

            foreach (var item in pans) {
                item.Visibility = Visibility.Hidden;
            }
            pans[i].Visibility = Visibility.Visible;
        }

        private static bool IsTextAllowed(string text) {
            Regex regex = new Regex("^[0-9]+$");
            return regex.IsMatch(text);
        }

        void SetTrayMonitorSelection(int selection) {
            foreach (WinForms.MenuItem item in trayiconMonitor.MenuItems) {
                item.Checked = false;
            }
            trayiconMonitor.MenuItems[selection].Checked = true;
        }

        void SetTrayModeSelection(int selection) {
            foreach (WinForms.MenuItem item in trayiconMode.MenuItems) {
                item.Checked = false;
            }
            trayiconMode.MenuItems[selection].Checked = true;
        }

        #endregion


        #region Trayicon Eventhandlers

        private async void trayicon_RefreshStreamInfo(object obj, EventArgs e) {
            ConfigMgnr.I.NotifyController.StopSound();
            await ConfigMgnr.I.NotifyController.DataHandler.Update();
            ConfigMgnr.I.NotifyController.DisplayNotification();
        }

        private void trayicon_OpenMainWindow(object sender, EventArgs e) {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.WindowState.Normal;
        }

        private void trayicon_Exit(object sender, EventArgs e) {
            this.Close();
        }

        private async void trayicon_Click(object sender, WinForms.MouseEventArgs e) {
            if (e.Button == WinForms.MouseButtons.Left) {
                ConfigMgnr.I.NotifyController.StopSound();
                // Special case when NoAlerts is active and nothing gets pulled by the updater thread
                if (NotifyCtr.NotifyModes.NoAlerts == (NotifyCtr.NotifyModes)ConfigMgnr.I.Mode) {
                    var s = ConfigMgnr.I.NotifyController.DataHandler.CurrentStreams;
                    if (s == null) {
                        await ConfigMgnr.I.NotifyController.DataHandler.Update();
                    }
                    else if (DateTime.Now.Subtract(s.CreationTime).TotalSeconds > (double)ConfigMgnr.I.UpdateFrequency) {
                        // Prevent misuse of the api by spamming the button
                        await ConfigMgnr.I.NotifyController.DataHandler.Update();
                    }
                }
                ConfigMgnr.I.NotifyController.DisplayNotification();
            }
            else if (e.Button == WinForms.MouseButtons.Middle) {
                trayicon_OpenMainWindow(this, EventArgs.Empty);
            }
        }

        private void trayicon_MonitorItem(object sender, EventArgs e) {
            var m = (WinForms.MenuItem)sender;
            int selection = int.Parse(m.Text);
            SetTrayMonitorSelection(selection);
            dropMonitorSelect.SelectedIndex = selection;
            ConfigMgnr.I.DisplayMonitor = selection;
        }

        private void trayicon_OnModeClick(object sender, EventArgs e) {
            var s = (WinForms.MenuItem)sender;
            ConfigMgnr.I.Mode = s.Index;
            SetTrayModeSelection(s.Index);
            dropModeSelect.SelectedIndex = s.Index;
        }

        #endregion


        #region Eventhandlers

        private void Window_StateChanged(object sender, EventArgs e) {
            if (System.Windows.WindowState.Minimized == this.WindowState) {
                this.Hide();
                this.ShowInTaskbar = false;
            }
        }

        private void btnSoundSelect_Click(object sender, RoutedEventArgs e) {
            System.IO.Stream soundFile = null;
            WinForms.OpenFileDialog opf = new WinForms.OpenFileDialog();

            opf.InitialDirectory = "c:\\";
            opf.Filter = "sound files (*.wav)|*.wav";
            opf.FilterIndex = 1;
            opf.RestoreDirectory = true;

            if (opf.ShowDialog() == WinForms.DialogResult.OK) {
                try {
                    if ((soundFile = opf.OpenFile()) != null) {
                        using (var fileStream = new System.IO.FileStream(ConfigMgnr.I.FolderPath + @"\sound.wav", System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
                            soundFile.CopyTo(fileStream);
                        }
                    }
                }
                catch (Exception) {

                }
            }
        }

        private void DevConsoleInput_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Acon.In = DevConsoleInput.Text;
                DevConsoleInput.Clear();
            }
        }

        private void Navbar_Click(object sender, MouseButtonEventArgs e) {
            var s = (TextBlock)sender;
            var defCol = (SolidColorBrush)FindResource("Color_DefaultText");
            var actCol = (SolidColorBrush)FindResource("Color_Highlight");
            TextBlock[] navBtns = { btn_Follows, btn_Settings };

            for (int i = 0; i < navBtns.Length; i++) {
                if (navBtns[i].Equals(s)) {
                    SetActivePanel(i);
                    navBtns[i].Foreground = actCol;
                }
                else
                    navBtns[i].Foreground = defCol;
            }

            if (s.Equals(btn_Follows))
                UpdateFollowsList();
        }

        private void btnConfigPath_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start(ConfigMgnr.I.FolderPath);
        }

        private void chkWin_Checked(object sender, RoutedEventArgs e) {
            ConfigMgnr.I.StartWithWindows = (bool)chkWin.IsChecked;

            MiscOperations.SetRegistryStartup((bool)chkWin.IsChecked);
        }

        private void chkMin_Checked(object sender, RoutedEventArgs e) {
            ConfigMgnr.I.StartMinimized = (bool)chkMin.IsChecked;
        }

        private void PreviewInputBoxNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void boxUpdFreq_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Keyboard.ClearFocus();
            }
        }

        private void boxPopTime_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                Keyboard.ClearFocus();
            }
        }

        private void dropModeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ConfigMgnr.I.Mode = dropModeSelect.SelectedIndex;
            SetTrayModeSelection(dropModeSelect.SelectedIndex);
        }

        private void dropMonitorSelect_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SetTrayMonitorSelection(dropMonitorSelect.SelectedIndex);
            ConfigMgnr.I.DisplayMonitor = dropMonitorSelect.SelectedIndex;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.F12) {
                SetActivePanel(2);
                DevConsoleInput.Focus();
            }
        }

        private void hyberlinkFollowProfile_OnClick(object sender, MouseButtonEventArgs e) {
            var obj = (TextBlock)sender;
            var obj2 = (StackPanel)obj.Parent;
            var obj3 = (DockPanel)obj2.Parent;
            var textB = (TextBlock)VisualTreeHelper.GetChild(obj3, 0);
            if (ConfigMgnr.I.OpenStreamWithScript) {
                System.Diagnostics.Process.Start(string.Format(@"{0}\{1}", ConfigMgnr.I.FolderPath, MiscOperations.StreamFileName),
                textB.Text);
            }
            else {
                System.Diagnostics.Process.Start("https://twitch.tv/" + textB.Text);
            }
        }

        private void hyberlinkFollowVod_OnClick(object sender, MouseButtonEventArgs e) {
            var obj = (TextBlock)sender;
            var obj2 = (StackPanel)obj.Parent;
            var obj3 = (DockPanel)obj2.Parent;
            var textB = (TextBlock)VisualTreeHelper.GetChild(obj3, 0);
            var vodLink = string.Format("{0}/{1}", textB.Text, "videos?filter=archives&sort=time");
            if (ConfigMgnr.I.OpenStreamWithScript) {
                System.Diagnostics.Process.Start(string.Format(@"{0}\{1}", ConfigMgnr.I.FolderPath, MiscOperations.StreamFileName),
                vodLink);
            }
            else {
                System.Diagnostics.Process.Start("https://twitch.tv/" + vodLink);
            }
        }

        private void sourceCodeLink_MouseDown(object sender, MouseButtonEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/MartinHartmannJensen/TwitchNotifier#arethrunotifier");
        }

        private void chkScript_Click(object sender, RoutedEventArgs e) {
            ConfigMgnr.I.OpenStreamWithScript = (bool)chkScript.IsChecked;
        }

        private void boxUpdFreq_TextChanged(object sender, TextChangedEventArgs e) {
            var s = (TextBox)sender;
            int numba;
            int.TryParse(s.Text, out numba);
            if (numba < 10)
                numba = 10;
            ConfigMgnr.I.UpdateFrequency = numba;
        }

        private void boxPopTime_TextChanged(object sender, TextChangedEventArgs e) {
            var s = (TextBox)sender;
            int numba;
            int.TryParse(s.Text, out numba);
            if (numba < 3)
                numba = 3;
            ConfigMgnr.I.NotificationScreenTime = numba;
        }

        private async void btnAuth_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start(Helix.HelixAPI.AuthURL);
            string user_tok = await Helix.HelixAPI.ListenForResponse();
            ConfigMgnr.I.Token = user_tok;
            WinForms.Application.Restart();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void btnUninstall_Click(object sender, RoutedEventArgs e) {
            SetActivePanel(3);
        }
        #endregion

        private void btnGotosettings_Click(object sender, RoutedEventArgs e) {
            SetActivePanel(1);
        }

        private void btnRemoveSettings_Click(object sender, RoutedEventArgs e) {
            MiscOperations.SetRegistryStartup(false);
            MiscOperations.RemoveConfig();
            WinForms.Application.Exit();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
