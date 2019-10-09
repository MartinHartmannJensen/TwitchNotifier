using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using ArethruNotifier.Helix;

namespace ArethruNotifier {
    public partial class NotificationWindow {
        private int monitorIndex = 0;
        public int MonitorIndex { get { return monitorIndex; } set { monitorIndex = value; } }


        public NotificationWindow() {
            InitializeComponent();
            SetPosition();
            btnClose.Effect = null;
        }

        public void SetPosition() {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => {
                var workingArea = WinForms.Screen.AllScreens[MonitorIndex].WorkingArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                this.Left = corner.X - this.ActualWidth - 100;
                this.Top = corner.Y - this.ActualHeight;
            }));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Storyboard_Completed(object sender, EventArgs e) {
            this.Close();
        }

        private void listDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListView listV = (ListView)sender;

            var selected = (Stream)listV.Items[listV.SelectedIndex];
            string selectedName = selected.Channel.ToLower();

            if (!ConfigMgnr.I.OpenStreamWithScript) {
                System.Diagnostics.Process.Start("http://www.twitch.tv/" + selectedName);
            }
            else {
                System.Diagnostics.Process.Start(string.Format(@"{0}\{1}", ConfigMgnr.I.FolderPath, MiscOperations.StreamFileName),
                selectedName);
            }
            this.Close();
        }

    }
}
