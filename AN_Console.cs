using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArethruNotifier.Helix;

namespace ArethruNotifier {
    abstract class CmdNode {
        public delegate void PrintMethod(string s);
        protected PrintMethod OutM;
        protected CmdNode[] cmds;
        protected string Out { set { OutM(value + "\n"); } }

        public abstract void Main(PrintMethod p);
        public abstract void Execute(string[] args);
    }

    class CmdMain : CmdNode {
        public override void Main(PrintMethod p) {
            cmds = new CmdNode[] {
                new CmdGet()
            };
            foreach (var item in cmds) {
                item.Main(p);
            }
        }
        public override void Execute(string[] args) {
            var c = args[0];
            if (c.Equals("GET")) {
                cmds[0].Execute(args);
            }
        }
    }

    class CmdGet : CmdNode {
        public override void Main(PrintMethod p) {
            OutM = p;
        }
        public override void Execute(string[] args) {
            var c = args[1];
            if (args.Length > 2) {
                if (c.Equals("USER")) {
                    User(args[2]);
                }
                if (c.Equals("FOLLOWS")) {
                    Follows(args[2]);
                }
                if (c.Equals("STREAMS")) {
                    Streams(args[2]);
                }
            }
            else {
                Out = "More arguments needed!";
                Out = "GET OBJECT VALUE";
            }
        }

        private async void User(string param) {
            var user = await HelixAPI.GetUser(param);
            if (user.User.Count > 0) {
                Out = user.User[0].Id;
            }
        }

        private async void Follows(string param) {
            int i;
            if (!int.TryParse(param, out i)) {
                return;
            }
            var follows = await HelixAPI.GetFollows(param);
            Out = "[ Users Follows ]";
            foreach (var item in follows.Follow) {
                Out = item.Name;
            }
        }

        private async void Streams(string param) {
            int i;
            if (!int.TryParse(param, out i)) {
                return;
            }
            var follows = await HelixAPI.GetFollows(param);
            string ids = follows.GenerateUserIds();
            var streams = await HelixAPI.GetStreams(ids);
            Out = "[ Users Live Followed Streams ]";
            foreach (var item in streams.Stream) {
                Out = item.Channel;
            }
        }
    }

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

        CmdMain main;


        public AN_Console(System.Windows.Controls.RichTextBox textbox, MainWindow textboxOwner) {
            mainWin = textboxOwner;
            Acout = textbox.AppendText;
            Acclear = textbox.Document.Blocks.Clear;
            //SetTreeValue();
            main = new CmdMain();
            main.Main(textbox.AppendText);
        }

        void CommandParse(string s) {
            s = s.ToUpper();
            string[] cmds = s.Split(null);
            try {
                main.Execute(cmds);
            }
            catch (IndexOutOfRangeException) {
                Out = "Unknown Command";
            }
            

            //try {
            //    switch (cmds.Length) {
            //        case 1:
            //            mainTree["ONELINERS"][cmds[0]].Invoke();
            //            break;
            //        case 2:
            //            mainTree[cmds[0]][cmds[1]].Invoke();
            //            break;
            //        default:
            //            Out = errMsg;
            //            break;
            //    }
            //}
            //catch (IndexOutOfRangeException) {
            //    Out = errMsg;
            //}
            //catch (KeyNotFoundException) {
            //    Out = errMsg;
            //}
        }

        //void SetTreeValue() {
        //    mainTree["GET"] = get;
        //    mainTree["ONELINERS"] = oneliners;

        //    get["STREAMS"] = new Action(async () => {
        //        // get follows -> get streams from user ids -> display live
        //        Helix.Follows folls = await Helix.HelixAPI.GetFollows("68744599", 100);
        //        var idstr = folls.GenerateUserIds();
        //        Helix.Streams strims = await Helix.HelixAPI.GetStreams(idstr);

        //        foreach (var item in strims.Stream) {
        //            if (item.IsLive) {
        //                Out = item.Channel;
        //            }
        //        }
        //    });

        //    get["FOLLOWS"] = new Action(async () => {
        //        Helix.Follows valhalla = await HelixAPI.GetFollows("68744599");
        //        Out = "[ Users Follows ]";
        //        foreach (var item in valhalla.Follow) {
        //            Out = item.Name;
        //        }
        //    });

        //    oneliners["CLEAR"] = new Action(() => {
        //        Acclear();
        //    });

        //    oneliners["SOUND"] = new Action(() => {
        //        ConfigMgnr.I.NotifyController.PlaySound();
        //    });

        //    oneliners["USER"] = new Action(async () => {
        //        var user = await HelixAPI.GetUser("Arethru");
        //        if (user.User.Count > 0) {
        //            Out = user.User[0].Id;
        //        }
        //    });
        //}
    }
}
