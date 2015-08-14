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
        #region Fields
        NotifyIcon notifyIcon1;
        public NotificationWindow noteW;
        Thread nuT;
        public MyNotificationEventHandler NotifyEvent;
        FormSettings sForm;

        StreamsInfo streamInfo = null;
        #endregion

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
            notifyIcon1.MouseDoubleClick += NotifyIcon1_DoubleClick;

            this.FormClosed += Form1_FormClosed;
            this.NotifyEvent += NotificationReceived;

            noteW = null;
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
            }

        }

        public void InvokeNotification(EventArgs e)
        {
            if (NotifyEvent != null)
                NotifyEvent(this, e);
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(nuT != null && nuT.IsAlive)
                nuT.Abort();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetLiveStreamsSetText();
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

        private void GetLiveStreamsSetText()
        {
            richTextBox1.Clear();
            var data = RESTcall.GetLiveStreams();

            if (data.isSucces)
            {
                streamInfo = data;

                StringBuilder sb = new StringBuilder();

                foreach (var item in data.Streams)
                {
                    sb.AppendLine(item.Channel.Name);
                    sb.AppendLine("Viewers: " + item.Viewers);
                    sb.AppendLine(item.Game);
                    sb.AppendLine(item.Channel.Url);
                    sb.AppendLine();
                }

                richTextBox1.Clear();
                richTextBox1.Text = sb.ToString();
                return;
            }

            richTextBox1.Text = "Something went wrong";
        }

        private void LiveStreamsSetText()
        {
            var data = streamInfo;

            StringBuilder sb = new StringBuilder();

            foreach (var item in data.Streams)
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

        private void btnXamlWindow_Click(object sender, EventArgs e)
        {
            InvokeNotification(EventArgs.Empty);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.Hide();
              
            }
            else if(FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void NotifyIcon1_DoubleClick(object sender, MouseEventArgs e)
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

        /// <summary>
        /// Executes code On NotifyEvent. Gets data from RESTcall and determines if something new has happened.
        /// Displays the popup in case: true
        /// Will probably get cleaned up in the future and have functionality divivided in better manner
        /// </summary>
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
                    if (streamInfo.Streams.Exists(x => x.Channel.Name.Equals(item.Channel.Name)))
                    {

                    }
                    else
                    {
                        newChannelFound = true;
                    }
                }

                if (!newChannelFound)
                    return;
            }

            streamInfo = tempSI;

            LiveStreamsSetText();

            StringBuilder sb = new StringBuilder();
            foreach (var item in streamInfo.Streams)
            {
                sb.AppendLine(item.Channel.Name);
                sb.AppendLine("playing " + item.Game);
                sb.AppendLine();
            }

            if (noteW != null)
                noteW.Close();
            noteW = new NotificationWindow();
            noteW.ShowInTaskbar = false;
            noteW.StreamList.Text = sb.ToString();
            int windowseconds = Settings.Default.WindowTimeOnScreen;
            noteW.WindowTimeOnScreen.KeyTime = new TimeSpan(0, 0, windowseconds);
            noteW.WindowTimeOnScreen2.KeyTime = new TimeSpan(0, 0, windowseconds + 2);
            noteW.Show();
        }
    }
}
