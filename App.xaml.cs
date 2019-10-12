using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace ArethruNotifier {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public App() {
            this.Startup += App_Startup;
            this.Exit += App_Exit;

            // TODO search on up to date solution
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Init ConfigMgnr
            var test = ConfigMgnr.I.ConfigPath;

            MiscOperations.CreateStreamLaunchFile(ConfigMgnr.I.FolderPath);
            MiscOperations.CreateFavoriteConfig(ConfigMgnr.I.FolderPath);
            MiscOperations.SetRegistryStartup(ConfigMgnr.I.StartWithWindows);

            // Dynamically set colors from config
            try {
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
            catch (Exception) {
                // Should be fine :)
            }
        }

        private void App_Startup(object sender, StartupEventArgs e) {
            ConfigMgnr.I.NotifyController.StartStreaminfoUpdater();
        }

        private void App_Exit(object sender, ExitEventArgs e) {
            ConfigMgnr.I.NotifyController.StopStreamInfoUpdater();
        }
    }
}
