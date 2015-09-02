﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArethruTwitchNotifier
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow
    {
        public NotificationWindow()
        {
            InitializeComponent();
            SetPosition();
            this.TextTime.Text = "recieved at " + StreamContainer.Instance.TimeRecieved.ToShortTimeString();
            btnClose.Effect = null;
        }

        public void SetPosition()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                this.Left = corner.X - this.ActualWidth - 100;
                this.Top = corner.Y - this.ActualHeight;
            }));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MyThreading.Instance.StopSound();
            this.Close();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listV = (ListView)sender;

            var myItem = (StreamsObj)listV.Items[listV.SelectedIndex];
            
            System.Diagnostics.Process.Start("http://www.twitch.tv/" + myItem.Channel.Name);

            this.Close();
        }

    }
}
