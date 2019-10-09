using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArethruNotifier.Helix;

namespace ArethruNotifier {
    // Work in progress, just a quick take on a commandline structure 
    class AN_Console {
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


        public AN_Console(System.Windows.Controls.RichTextBox textbox, MainWindow textboxOwner) {
            mainWin = textboxOwner;
            Acout = textbox.AppendText;
            Acclear = textbox.Document.Blocks.Clear;
            SetTreeValue();
        }

        void CommandParse(string s) {
            string errMsg = "Unknown Command";
            s = s.ToUpper();
            string[] cmds = s.Split(null);

            try {
                switch (cmds.Length) {
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
            catch (IndexOutOfRangeException) {
                Out = errMsg;
            }
            catch (KeyNotFoundException) {
                Out = errMsg;
            }
        }

        void SetTreeValue() {
            mainTree["GET"] = get;
            mainTree["ONELINERS"] = oneliners;

            get["KLIVE"] = new Action(() => {
                var val = Kraken.APIcalls.GetLiveStreams();
                Out = "[ Live ]";
                foreach (var item in val.Stream) {
                    Out = item.Channel.Name;
                }
            });

            get["HUSER"] = new Action(async () => {
                Users val = await HelixAPI.GetUser("baertaffy");
                Out = "[ User Id ]";
                foreach (var item in val.User) {
                    Out = item.Id;
                }
            });

            get["STREAMS"] = new Action(async () => {
                // get follows -> get streams from user ids -> display live
                Helix.Follows folls = await Helix.HelixAPI.GetFollows("68744599", 100);
                var idstr = folls.GenerateUserIds();
                Helix.Streams strims = await Helix.HelixAPI.GetStreams(idstr);

                foreach (var item in strims.Stream) {
                    if (item.IsLive) {
                        Out = item.Channel;
                    }
                }
            });

            get["HELIXFOL"] = new Action(async () => {
                // TODO i dunno null error :)
                Helix.Follows valhalla = await HelixAPI.GetFollows("68744599");
                Out = "[ Users Follows ]";
                foreach (var item in valhalla.Follow) {
                    Out = item.Name;
                }
            });

            get["FOLLOWS"] = new Action(() => {
                var val = Kraken.APIcalls.GetFollowedChannels();
                Out = "[ Follows ]";
                foreach (var item in val.List) {
                    Out = item.Channel.Name + item.Channel.Logo;
                }
            });

            oneliners["TEST"] = new Action(() => {
                var val = Kraken.APIcalls.GetLiveStreams();
                if (val.IsSuccess)
                    Out = "Test> Connected";
                else
                    Out = "Test> Connection Failed [DebugMessage]: " + val.DebugMessage;
            });

            oneliners["CLEAR"] = new Action(() => {
                Acclear();
            });

            oneliners["POP"] = new Action(() => {
                ConfigMgnr.I.NotifyController.DisplayNotificationWindow();
            });

            oneliners["LIVEPOP"] = new Action(() => {
                ConfigMgnr.I.DataHandler.UpdateLive(TwitchDataHandler.UpdateMode.Force);
            });

            oneliners["FOLLOWS"] = new Action(() => {
                mainWin.UpdateFollowsList();
                Out = "Follows tab populated";
            });

            oneliners["SOUND"] = new Action(() => {
                ConfigMgnr.I.NotifyController.PlaySound();
            });
        }
    }
}
