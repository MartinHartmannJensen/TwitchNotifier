using System;
using System.Threading;
using WinForms = System.Windows.Forms;

namespace ArethruNotifier {
    public class NotifyCtr {

        Thread updater;
        Thread windowThread;
        System.Media.SoundPlayer player;

        string soundPath = ConfigMgnr.I.FolderPath + @"\sound.wav";

        public void StartStreaminfoUpdater() {
            if (updater != null && updater.IsAlive)
                updater.Abort();

            updater = new Thread(new ThreadStart(() => {
                while (true) {
                    ConfigMgnr.I.DataHandler.UpdateLive(TwitchDataHandler.UpdateMode.Compare);
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
        public void DisplayNotificationWindow() {
            CreateWindow(ConfigMgnr.I.DataHandler.CurrentInfo, ConfigMgnr.I.NotificationScreenTime);
        }
        /// <summary>
        /// Create a popupwindow with provided StreamsInfo
        /// </summary>
        /// <param name="si">Data for the window</param>
        public void DisplayNotificationWindow(StreamsInfo si) {
            CreateWindow(si, ConfigMgnr.I.NotificationScreenTime);
            PlaySound();
        }
        /// <summary>
        /// Create a popupwindow with provided StreamsInfo with the settings of the selected FavouriteGroup
        /// </summary>
        /// <param name="si"></param>
        /// <param name="fg"></param>
        public void DisplayNotificationWindow(StreamsInfo si, FavouriteGroup fg) {
            CreateWindow(si, fg.Poptime);
            PlaySound(fg.Soundfile);
        }

        private void CreateWindow(StreamsInfo sInfo, int displayTime) {
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
                if (sInfo != null && sInfo.Streams != null && sInfo.Streams.Count > 0) {
                    w.TextTime.Text = "recieved at " + sInfo.CreationTime.ToShortTimeString();
                    w.listDataBinding.ItemsSource = sInfo.Streams;
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
            if (ConfigMgnr.I.PlaySound) {
                try {
                    player = new System.Media.SoundPlayer(soundPath);
                    player.Play();
                }
                catch (System.IO.FileNotFoundException) {
                    player = new System.Media.SoundPlayer(Properties.Resources.nSound);
                    player.Play();
                }
            }
        }

        public void PlaySound(string favouriteSound) {
            if (ConfigMgnr.I.PlaySound) {
                try {
                    player = new System.Media.SoundPlayer(ConfigMgnr.I.FolderPath + @"\group sounds\" + favouriteSound);
                    player.Play();
                }
                catch (System.IO.FileNotFoundException) {
                    player = new System.Media.SoundPlayer(Properties.Resources.nSound);
                    player.Play();
                }
            }
        }

        public void StopSound() {
            if (player != null)
                player.Stop();
        }
    }
}
