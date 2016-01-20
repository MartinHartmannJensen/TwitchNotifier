using System;
using System.Threading;

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
        Thread soundThread;
        System.Media.SoundPlayer player;


        private NotifyCtr()
        {
            TwitchDataHandler.Instance.FoundNewStreamEvent += new NewStreamFoundEventHandler((StreamsInfo si) => {
                DisplayNotification(si, UserSettings.Default.NotificationScreenTime);
            });
        }

        public void StartStreamContainerUpdater()
        {
            if (updater != null && updater.IsAlive)
                updater.Abort();

            updater = new Thread(new ThreadStart(() =>
            {
                var seconds = UserSettings.Default.UpdateFrequency;

                Thread.Sleep(1000);

                while (true)
                {
                    //if (NotifyEvent != null)
                    //    NotifyEvent(null, EventArgs.Empty);

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
                    var testArrayOutOfBounds = System.Windows.Forms.Screen.AllScreens[UserSettings.Default.DisplayMonitor];
                    w.MonitorIndex = UserSettings.Default.DisplayMonitor;
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

        public void PlaySound(string name)
        {
            //if (Properties.Settings.Default.PlaySound)
            if (UserSettings.Default.PlaySound)
            {
                if (soundThread != null && soundThread.IsAlive)
                    soundThread.Abort();

                soundThread = new Thread(new ThreadStart(() =>
                {
                    string soundName = name;

                    string soundPath = Environment.CurrentDirectory + @"\" + soundName;

                    try
                    {
                        player = new System.Media.SoundPlayer(soundPath);
                        player.Play();
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    catch (System.InvalidOperationException)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                }));

                soundThread.IsBackground = true;
                soundThread.Start();
            }
        }

        public void StopSound()
        {
            if (player != null)
                player.Stop();
        }
    }
}
