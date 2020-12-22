using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace Shos.Chatter.Wpf
{
    public partial class App : Application
    {
        public class AppSettings
        {
            public string ServerUrl { get; set; } = "";
        }

        const string settingFileName = "appsettings.json";

        public static AppSettings Settings { get; } = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                                .AddJsonFile(settingFileName, optional: true, reloadOnChange: true)
                                                                                .Build()
                                                                                .Get<AppSettings>();
    }
}
