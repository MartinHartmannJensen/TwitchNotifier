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
    }
}
