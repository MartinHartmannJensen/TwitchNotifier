using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using System.Windows.Media;

namespace ArethruNotifier
{
    public partial class NotificationWindow
    {
        private int monitorIndex = 0;
        public int MonitorIndex { get { return monitorIndex; } set { monitorIndex = value; } }


        public NotificationWindow()
        {
            InitializeComponent();
            SetPosition();
            this.TextTime.Text = "recieved at " + TwitchDataHandler.Instance.TimeRecieved.ToShortTimeString();
            btnClose.Effect = null;

            this.Closing += new System.ComponentModel.CancelEventHandler((object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                NotifyCtr.Instance.StopSound();
            });

            if (!ConfigMgnr.I.Color_MainPanel.Equals("0"))
                Resources["Color_MainPanel"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_MainPanel));
            if (!ConfigMgnr.I.Color_SubPanel.Equals("0"))
                Resources["Color_SubPanel"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_SubPanel));
            if (!ConfigMgnr.I.Color_DefaultText.Equals("0"))
                Resources["Color_DefaultText"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_DefaultText));
            if (!ConfigMgnr.I.Color_Highlight.Equals("0"))
                Resources["Color_Highlight"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_Highlight));
            if (!ConfigMgnr.I.Color_BtnBG.Equals("0"))
                Resources["Color_BtnBG"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(ConfigMgnr.I.Color_BtnBG));
        }

        public void SetPosition()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = WinForms.Screen.AllScreens[MonitorIndex].WorkingArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                this.Left = corner.X - this.ActualWidth - 100;
                this.Top = corner.Y - this.ActualHeight;
            }));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listV = (ListView)sender;

            var selected = (StreamsObj)listV.Items[listV.SelectedIndex];
            string selectedName = selected.Channel.Name.ToLower();

            if (!ConfigMgnr.I.OpenStreamWithScript)
            {
                System.Diagnostics.Process.Start("http://www.twitch.tv/" + selectedName);
            }
            else
            {
                System.Diagnostics.Process.Start(string.Format(@"{0}\{1}", ConfigMgnr.I.FolderPath, MiscOperations.StreamFileName),
                selectedName);
            }
            this.Close();
        }

    }
}
