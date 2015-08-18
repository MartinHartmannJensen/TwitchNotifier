using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArethruTwitchNotifier
{
    public delegate void MyNotificationEventHandler(object sender, EventArgs e);

    public partial class Form1 : Form
    {
        public MyNotificationEventHandler NotifyEvent;
        NotifyIcon notifyIcon1;
        Thread nuT;
        FormSettings sForm;
        StreamsInfo streamInfo = null;
        Thread notifyWindowThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1 = new NotifyIcon();
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.BalloonTipText = "ArethruTwitchNotifier";
            notifyIcon1.Text = "ArethruTwitchNotifier";
            notifyIcon1.MouseDoubleClick += NotifyIcon1_Click;
            notifyIcon1.ContextMenu = new ContextMenu(new MenuItem[4] 
            { new MenuItem("Sound", NotifyIcon1_Sound), new MenuItem("Refresh and Show", btnXamlWindow_Click), 
                new MenuItem("Show", NotifyIcon1_Show), new MenuItem("Exit", Form1_Close) });
            notifyIcon1.ContextMenu.MenuItems[0].Checked = Settings.Default.PlaySound;



            this.FormClosed += Form1_FormClosed;
            this.NotifyEvent += NotificationReceived;

            sForm = null;

            if (Settings.Default.RunAutoUpdateAtStart)
            {
                MyThreading mtc = new MyThreading();
                nuT = new Thread(mtc.ScheduledLivestreamUpdate);
                nuT.Start(this);
            }

            if (Settings.Default.StartMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void NotifyIcon1_Show(object sender, EventArgs e)
        {
            DisplayNotification(streamInfo);
        }

        private void NotifyIcon1_Sound(object sender, EventArgs e)
        {
            var s = (MenuItem)sender;

            if (!s.Checked)
                s.Checked = true;
            else
                s.Checked = false;


            Settings.Default.PlaySound = s.Checked;
            Settings.Default.Save();
        }

        public void InvokeNotification(EventArgs e)
        {
            if (NotifyEvent != null)
                NotifyEvent(this, e);
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (nuT != null && nuT.IsAlive)
                nuT.Abort();
        }

        void Form1_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            streamInfo = RESTcall.GetLiveStreams();

            LiveStreamsSetText(streamInfo);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void btnGetResponseString_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Text = RESTcall.GetLiveStreamsFullString();
        }

        private void btnXamlWindow_Click(object sender, EventArgs e)
        {
            streamInfo = RESTcall.GetLiveStreams();

            DisplayNotification(streamInfo);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.Hide();

            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void NotifyIcon1_Click(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (sForm != null)
                sForm.Close();

            sForm = new FormSettings();
            sForm.Show();
        }

        private void NotificationReceived(object sender, EventArgs e)
        {
            var tempSI = RESTcall.GetLiveStreams();

            if (!tempSI.isSucces)
            {
                richTextBox1.Text = "Something went wrong, check your connection and make sure that you have configured your User Token in Settings";
                return;
            }

            if (streamInfo != null)
            {
                bool newChannelFound = false;
                foreach (var item in tempSI.Streams)
                {
                    if (!streamInfo.Streams.Exists(x => x.Channel.Name.Equals(item.Channel.Name)))
                        newChannelFound = true;
                }

                if (!newChannelFound)
                {
                    streamInfo = tempSI;
                    LiveStreamsSetText(tempSI);
                    return;
                }
            }

            streamInfo = tempSI;
            LiveStreamsSetText(tempSI);
            DisplayNotification(tempSI);

            if (Settings.Default.PlaySound)
                PlaySound("nSound.wav");
        }

        private void DisplayNotification(StreamsInfo sInfo)
        {
            if (notifyWindowThread != null && notifyWindowThread.IsAlive)
                notifyWindowThread.Abort();

            notifyWindowThread = new Thread(new ThreadStart(() => {
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

            notifyWindowThread.SetApartmentState(ApartmentState.STA);
            notifyWindowThread.IsBackground = true;
            notifyWindowThread.Start();
        }

        private void PlaySound(string name)
        {
            string soundName = name;

            string soundPath = Environment.CurrentDirectory + @"\" + soundName;

            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(soundPath);
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
        }

        private void LiveStreamsSetText(StreamsInfo si)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in si.Streams)
            {
                sb.AppendLine(item.Channel.Name);
                sb.AppendLine("Viewers: " + item.Viewers);
                sb.AppendLine(item.Game);
                sb.AppendLine(item.Channel.Url);
                sb.AppendLine();
            }

            richTextBox1.Clear();
            richTextBox1.Text = sb.ToString();
        }
    }
}
