using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Yapped.Klaus.Core.Data;

namespace Yapped.Klaus.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppConfiguration? appConfiguration = null;

            if (File.Exists(AppConfiguration.AppConfigSavePath))
            {
                try
                {
                    appConfiguration = JsonSerializer.Deserialize<AppConfiguration>(
                        File.ReadAllText(
                            AppConfiguration.AppConfigSavePath
                            )
                        );
                }
                catch (Exception)
                {
                    Console.WriteLine("File Not Found");
                }
            }

            if (appConfiguration is null)
            {
                OpenFileDialog fileDialog = new()
                {
                    FileName = "gameparam.parambnd.dcx",
                    CheckFileExists = true,
                    Filter = "gameparams (*.parambnd.dcx)|*.parambnd.dcx",
                    Title = "Select gameparam.parambnd.dcx for Sekiro"
                };

                var fileName = string.Empty;
                
                var result = fileDialog.ShowDialog() ?? false;

                if (result)
                {
                    fileName = fileDialog.FileName;
                }

                appConfiguration = new AppConfiguration(fileName);

            }

            Helpers.RegisterServices(appConfiguration);

            if (Directory.Exists(AppConfiguration.LocalAppDataPath) is false)
            {
                Directory.CreateDirectory(AppConfiguration.LocalAppDataPath);
            }
            
            File.WriteAllText(AppConfiguration.AppConfigSavePath,
                JsonSerializer.Serialize(appConfiguration));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var appConfiguration = Ioc.Default.GetService<AppConfiguration>()!;

            if (Directory.Exists(AppConfiguration.LocalAppDataPath) is false)
            {
                Directory.CreateDirectory(AppConfiguration.LocalAppDataPath);
            }

            File.WriteAllText(AppConfiguration.AppConfigSavePath,
                JsonSerializer.Serialize(appConfiguration));
        }
    }
}
