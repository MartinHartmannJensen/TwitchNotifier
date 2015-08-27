using System;
using System.Collections.Generic;
using System.Linq;
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

        Thread windowThread;
        Thread soundThread;
        System.Media.SoundPlayer player;

        private MyThreading() { }

        public void ScheduledLivestreamUpdate(object obj)
        {
            var myForm = obj as Form1;

            var seconds = Settings.Default.UpdateFrequency;

            while (true)
            {
                myForm.BeginInvoke((Action)(() =>
                {
                    myForm.InvokeNotification(EventArgs.Empty);
                }));

                Thread.Sleep(seconds * 1000);
            }
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
                int windowseconds = Settings.Default.WindowTimeOnScreen;
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
            if (Settings.Default.PlaySound)
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
