using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArethruNotifier
{
    // Work in progress, just a quick take on a commandline structure 
    class AN_Console
    {
        MainWindow mainWin;

        // Made to hold a RichTextBox's Write and Clear methods
        public delegate void PrintMethod(string s);
        PrintMethod Acout;
        public delegate void ClearMethod();
        ClearMethod Acclear;

        // Pushes a string to the Output display (richtextbox)
        public string Out { set { Acout(value + "\n"); } }

        // Recieves an unformatted string with commands
        public string In { set { CommandParse(value); } }

        // Under consideration for change. These hold "Action"'s that are invoked in CommandParse(string) and set in SetTreeValue()
        Dictionary<string, Dictionary<string, Action>> mainTree = new Dictionary<string, Dictionary<string, Action>>();
        Dictionary<string, Action> get = new Dictionary<string, Action>();
        Dictionary<string, Action> oneliners = new Dictionary<string, Action>();


        public AN_Console(System.Windows.Controls.RichTextBox textbox, MainWindow textboxOwner)
        {
            mainWin = textboxOwner;
            Acout = textbox.AppendText;
            Acclear = textbox.Document.Blocks.Clear;
            SetTreeValue();
        }

        void CommandParse(string s)
        {
            string errMsg = "Unknown Command";
            s = s.ToUpper();
            string[] cmds = s.Split(null);

            try
            {
                switch (cmds.Length)
                {
                    case 1:
                        mainTree["ONELINERS"][cmds[0]].Invoke();
                        break;
                    case 2:
                        mainTree[cmds[0]][cmds[1]].Invoke();
                        break;
                    default:
                        Out = errMsg;
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Out = errMsg;
            }
            catch (KeyNotFoundException)
            {
                Out = errMsg;
            }
        }

        void SetTreeValue()
        {
            mainTree["GET"] = get;
            mainTree["ONELINERS"] = oneliners;

            get["LIVE"] = new Action(() => {
                var val = WebComm.GetLiveStreams();
                Out = "[ Live ]";
                foreach (var item in val.Streams)
                {
                    Out = item.Channel.Name;
                }
            });

            get["FOLLOWS"] = new Action(() => {
                var val = WebComm.GetFollowedChannels();
                Out = "[ Follows ]";
                foreach (var item in val.List)
                {
                    Out = item.Channel.Name;
                }
            });

            oneliners["TEST"] = new Action(() => {
                var val = WebComm.GetLiveStreams();
                if (val.IsSucces)
                    Out = "Test> Connected";
                else
                    Out = "Test> Connection Failed [DebugMessage]: " + val.DebugMessage;
            });

            oneliners["CLEAR"] = new Action(() => {
                Acclear();
            });

            oneliners["POP"] = new Action(() => {
                NotifyCtr.Instance.DisplayNotification(new StreamsInfo(), UserSettings.Default.NotificationScreenTime);
            });

            oneliners["LIVEPOP"] = new Action(() => {
                var si = WebComm.GetLiveStreams();
                NotifyCtr.Instance.DisplayNotification(si, UserSettings.Default.NotificationScreenTime);
            });

            oneliners["FOLLOWS"] = new Action(() => {
                mainWin.UpdateFollowsList();
                Out = "Follows tab populated";
            });

            oneliners["SCMD"] = new Action(() => {
                System.Diagnostics.Process.Start(@"C:\Users\Martin-PC\Desktop\streamstart.cmd", "somechannel");
            });

            oneliners["SOUND"] = new Action(() => {
                NotifyCtr.Instance.PlaySound();
            });
        }
    }
}
