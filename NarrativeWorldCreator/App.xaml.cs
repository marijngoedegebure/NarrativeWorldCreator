using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Semantics.Data;

namespace NarrativeWorldCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SystemStateTracker.Start = DateTime.Now;
        }
    }
}
