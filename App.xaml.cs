using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ArethruNotifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SetColors();
        }

        void SetColors()
        {
            string testcolor = "#25CF39";

            Resources["Color_DefaultText"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(testcolor));

        }
    }
}
