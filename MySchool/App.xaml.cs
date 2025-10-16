using System.Configuration;
using System.Data;
using System.Windows;
using System;
using System.IO;

namespace MySchool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mySchoolPath = Path.Combine(appData, "MySchool");
                if (!Directory.Exists(mySchoolPath))
                {
                    Directory.CreateDirectory(mySchoolPath);
                }
            }
            catch
            {
                // Ignore any IO exceptions on startup; app can still run without the folder
            }
        }
    }

}
