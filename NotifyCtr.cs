using System;
using System.Threading;
using WinForms = System.Windows.Forms;

namespace ArethruNotifier
{
    public class NotifyCtr
    {
        private static NotifyCtr instance = null;


        public static NotifyCtr Instance
        {
            get
            {
                if (instance == null)
                    instance = new NotifyCtr();
                return instance;
            }
        }

        Thread updater;
        Thread windowThread;
        System.Media.SoundPlayer player;

        string soundPath = ConfigMgnr.I.FolderPath + @"\sound.wav";


        private NotifyCtr()
        {
            TwitchDataHandler.Instance.FoundNewStreamEvent += new NewStreamFoundEventHandler((StreamsInfo si, FavouriteGroup fg) =>
            {
                if (fg == null)
                {
                    DisplayNotification(si, ConfigMgnr.I.NotificationScreenTime);
                    PlaySound();
                }
                else
                {
                    DisplayNotification(si, fg.Poptime);
                    PlaySound(fg.Soundfile);
                }
            });
        }

        public void StartStreaminfoUpdater()
        {
            if (updater != null && updater.IsAlive)
                updater.Abort();

            updater = new Thread(new ThreadStart(() =>
            {
                var seconds = ConfigMgnr.I.UpdateFrequency;

                Thread.Sleep(1000);

                while (true)
                {
                    var yup = WebComm.GetLiveStreams();
                    TwitchDataHandler.Instance.UpdateAndCompare(yup);

                    Thread.Sleep(seconds * 1000);
                }
            }));
            updater.IsBackground = true;
            updater.Start();
        }

        public void StopStreamContainerUpdater()
        {
            if (updater != null && updater.IsAlive)
                updater.Abort();
        }

        public void DisplayNotification(StreamsInfo sInfo, int displayTime)
        {
            if (windowThread != null && windowThread.IsAlive)
                windowThread.Abort();

            windowThread = new Thread(new ThreadStart(() =>
            {
                NotificationWindow w = new NotificationWindow();

                // Try and set display monitor
                try
                {
                    var testArrayOutOfBounds = System.Windows.Forms.Screen.AllScreens[ConfigMgnr.I.DisplayMonitor];
                    w.MonitorIndex = ConfigMgnr.I.DisplayMonitor;
                }
                catch (IndexOutOfRangeException)
                {
                    w.MonitorIndex = 0;
                }

                // Check if there's valid information to display
                if (sInfo != null && sInfo.Streams != null && sInfo.Streams.Count > 0)
                    w.listDataBinding.ItemsSource = sInfo.Streams;
                else
                    w.ErrorPanel.Visibility = System.Windows.Visibility.Visible;

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

        public void PlaySound()
        {
            if (ConfigMgnr.I.PlaySound)
            {
                try
                {
                    player = new System.Media.SoundPlayer(soundPath);
                    player.Play();
                }
                catch (System.IO.FileNotFoundException)
                {
                    player = new System.Media.SoundPlayer(Properties.Resources.nSound);
                    player.Play();
                }
            }
        }

        public void PlaySound(string favouriteSound)
        {
            if (ConfigMgnr.I.PlaySound)
            {
                try
                {
                    player = new System.Media.SoundPlayer(ConfigMgnr.I.FolderPath + @"\group sounds\" + favouriteSound);
                    player.Play();
                }
                catch (System.IO.FileNotFoundException)
                {
                    player = new System.Media.SoundPlayer(Properties.Resources.nSound);
                    player.Play();
                }
            }
        }

        public void StopSound()
        {
            if (player != null)
                player.Stop();
        }
    }
}
