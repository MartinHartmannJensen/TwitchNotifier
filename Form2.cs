using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArethruTwitchNotifier
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri(RESTcall.AuthURL);
            label1.Visible = false;
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //not used
            //string fullAuthstring = "http://localhost:4515/oauth2/authorize#access_token=123213213klsldakalsdasd&scope=user_follows_edit";


            var wbrow = (WebBrowser)sender;
            string theUrl = wbrow.Url.ToString();
            string compareURL = "http://localhost:4515/oauth2/authorize";
            

            if (theUrl.Substring(0, compareURL.Length).Equals(compareURL))
            {
                string trimStart = "http://localhost:4515/oauth2/authorize#access_token=";
                string trimEnd = "&scope=user_follows_edit";
                string trim1 = theUrl.TrimStart(trimStart.ToCharArray());
                string trim2 = trim1.TrimEnd(trimEnd.ToCharArray());

                textBox1.Text = trim2;
                label1.Visible = true;
                Settings.Default.UserToken = trim2;
                Settings.Default.Save();
            }
        }
    }
}
