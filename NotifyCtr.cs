using System;
using System.Threading;
using ArethruNotifier.Helix;
using TUR = ArethruNotifier.TwitchDataHandler.UpdateResult;

namespace ArethruNotifier {
    public class NotifyCtr {
        TwitchDataHandler tdh = null;
        public TwitchDataHandler DataHandler {
            get {
                return tdh;
            }
        }

        Thread updater;
        Thread windowThread;
        System.Media.SoundPlayer player;
        string soundPath = ConfigMgnr.I.FolderPath + @"\sound.wav";

        public enum NotifyModes {
            AllBells, OnlyFavorites, OnlyDisplayFavorites, NoSound, NoAlerts
        }

        public NotifyCtr() {
            tdh = new TwitchDataHandler();
        }

        public void StartStreaminfoUpdater() {
            if (updater != null && updater.IsAlive)
                updater.Abort();

            updater = new Thread(new ThreadStart(async () => {
                while (true) {
                    var m = (NotifyModes)ConfigMgnr.I.Mode;
                    if (m != NotifyModes.NoAlerts) {
                        TUR result = await tdh.Update();
                        Display(result, m);
                    }
                    Thread.Sleep(ConfigMgnr.I.UpdateFrequency * 1000);
                }
            }));
            updater.IsBackground = true;
            updater.Start();
        }

        public void StopStreamInfoUpdater() {
            if (updater != null && updater.IsAlive)
                updater.Abort();
        }

        /// <summary>
        /// Request a popupwindow be made from last gathered info
        /// </summary>
        public void DisplayNotification() {
            CreateWindow(tdh.CurrentStreams, ConfigMgnr.I.NotificationScreenTime);
        }

        /// <summary>
        /// Create a popupwindow with NotifyModes settings
        /// </summary>
        private void Display(TUR update, NotifyModes mode) {
            if (update == TUR.Nothing) {
                return;
            }
            int popT = ConfigMgnr.I.NotificationScreenTime;

            if (update == TUR.Update) {
                switch (mode) {
                    case NotifyModes.AllBells:
                        CreateWindow(tdh.CurrentStreams, popT);
                        PlaySound();
                        break;
                    case NotifyModes.NoSound:
                        CreateWindow(tdh.CurrentStreams, popT);
                        break;
                    default:
                        break;
                }
            }
            else {
                switch (mode) {
                    case NotifyModes.AllBells:
                    case NotifyModes.OnlyFavorites:
                        CreateWindow(tdh.CurrentStreams, tdh.CurrentFavorites.Poptime);
                        PlaySound(tdh.CurrentFavorites.Soundfile);
                        break;
                    case NotifyModes.OnlyDisplayFavorites:
                    case NotifyModes.NoSound:
                        CreateWindow(tdh.CurrentStreams, tdh.CurrentFavorites.Poptime);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CreateWindow(Streams sInfo, int displayTime) {
            if (windowThread != null && windowThread.IsAlive)
                windowThread.Abort();

            windowThread = new Thread(new ThreadStart(() => {
                NotificationWindow w = new NotificationWindow();

                w.Closing += new System.ComponentModel.CancelEventHandler((object sender, System.ComponentModel.CancelEventArgs e) => {
                    ConfigMgnr.I.NotifyController.StopSound();
                });


                // Try and set display monitor
                try {
                    var testArrayOutOfBounds = System.Windows.Forms.Screen.AllScreens[ConfigMgnr.I.DisplayMonitor];
                    w.MonitorIndex = ConfigMgnr.I.DisplayMonitor;
                }
                catch (IndexOutOfRangeException) {
                    w.MonitorIndex = 0;
                }

                // Check if there's valid information to display
                if (sInfo != null && sInfo.Stream != null && sInfo.Stream.Count > 0) {
                    w.TextTime.Text = "recieved at " + sInfo.CreationTime.ToShortTimeString();
                    w.listDataBinding.ItemsSource = sInfo.Stream;
                }
                else {
                    w.ErrorPanel.Visibility = System.Windows.Visibility.Visible;
                }


                w.ShowInTaskbar = false;
                w.WindowTimeOnScreen.KeyTime = new TimeSpan(0, 0, displayTime);
                w.WindowTimeOnScreen2.KeyTime = new TimeSpan(0, 0, displayTime + 2);

                w.Closed += (s, e) =>
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);

                w.Show();

                System.Windows.Threading.Dispatcher.Run();

            }));

            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.IsBackground = true;
            windowThread.Start();
        }

        public void PlaySound() {
            try {
                player = new System.Media.SoundPlayer(soundPath);
                player.Play();
            }
            catch (System.IO.FileNotFoundException) {
                player = new System.Media.SoundPlayer(Properties.Resources.nSound);
                player.Play();
            }
        }

        public void PlaySound(string favouriteSound) {
            try {
                player = new System.Media.SoundPlayer(ConfigMgnr.I.FolderPath + @"\group sounds\" + favouriteSound);
                player.Play();
            }
            catch (System.IO.FileNotFoundException) {
                player = new System.Media.SoundPlayer(Properties.Resources.nSound);
                player.Play();
            }
        }

        public void StopSound() {
            if (player != null)
                player.Stop();
        }
    }
}
