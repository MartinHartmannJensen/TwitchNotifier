using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArethruTwitchNotifier
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public delegate void MyNotificationEventHandler(object sender, EventArgs e);

    public class MyThreading
    {
        private static MyThreading instance = null;

        public static MyThreading Instance
        {
            get
            {
                if (instance == null)
                    instance = new MyThreading();
                return instance;
            }
        }

        public MyNotificationEventHandler NotifyEvent;

        Thread updater;
        Thread windowThread;
        Thread soundThread;
        System.Media.SoundPlayer player;

        private MyThreading() { }

        public void StartStreamContainerUpdater()
        {
            if (updater != null && updater.IsAlive)
                updater.Abort();

            updater = new Thread(new ThreadStart(() =>
            {
                //var seconds = Properties.Settings.Default.UpdateFrequency;
                var seconds = MHJ_ConfigManager.Settings.I.UpdateFrequency;

                Thread.Sleep(1000);

                while (true)
                {
                    //if (NotifyEvent != null)
                    //    NotifyEvent(null, EventArgs.Empty);

                    var yup = RESTcall.GetLiveStreams();
                    StreamContainer.Instance.UpdateAndCompare(yup);

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

        public void DisplayNotification(StreamsInfo sInfo)
        {
            if (windowThread != null && windowThread.IsAlive)
                windowThread.Abort();

            windowThread = new Thread(new ThreadStart(() =>
            {
                NotificationWindow w = new NotificationWindow();
                w.ShowInTaskbar = false;
                w.listDataBinding.ItemsSource = sInfo.Streams;
                //int windowseconds = Properties.Settings.Default.WindowTimeOnScreen;
                int windowseconds = MHJ_ConfigManager.Settings.I.WindowTimeOnScreen;
                w.WindowTimeOnScreen.KeyTime = new TimeSpan(0, 0, windowseconds);
                w.WindowTimeOnScreen2.KeyTime = new TimeSpan(0, 0, windowseconds + 2);

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
            if (MHJ_ConfigManager.Settings.I.PlaySound)
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

    public static class MiscOperation
    {
        public static void CreateBatRunFile()
        {
            if (!File.Exists(Environment.CurrentDirectory + @"\RunApplication.bat"))
            {
                using (StreamWriter sw = new StreamWriter("RunApplication.bat"))
                {
                    sw.Write(@"START /d " + '\u0022' + Environment.CurrentDirectory + '\u0022' + " ArethruTwitchNotifier.exe");
                }
            }
        }
    }

    
}
