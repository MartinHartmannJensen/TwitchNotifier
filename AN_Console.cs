using System;
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
                Out = "[GET OBJECT VALUE]";
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
    
    class AN_Console {
        MainWindow mainWin;

        // Clear Method Delegate
        public delegate void PrintMethod(string s);
        PrintMethod Acout;
        public delegate void ClearMethod();
        ClearMethod Acclear;

        // Pushes a string to the Output display (richtextbox)
        public string Out { set { Acout(value + "\n"); } }

        // Recieves an unformatted string with commands
        public string In { set { CommandParse(value); } }
        
        // The main guy. See abstract class CmdNode
        CmdMain main;


        public AN_Console(System.Windows.Controls.RichTextBox textbox, MainWindow textboxOwner) {
            mainWin = textboxOwner;
            Acout = textbox.AppendText;
            Acclear = textbox.Document.Blocks.Clear;
            main = new CmdMain();
            main.Main(textbox.AppendText);
        }

        void CommandParse(string s) {
            s = s.ToUpper();
            string[] cmds = s.Split(null);
            if (cmds[0].Equals("CLEAR")) {
                Acclear();
            }
            try {
                main.Execute(cmds);
            }
            catch (IndexOutOfRangeException) {
                Out = "Unknown Command";
            }
        }
    }
}
