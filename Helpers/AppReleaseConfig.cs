using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DesktopPortal.Helpers
{
    public class AppReleaseConfig
    {
        private static AppReleaseConfig? _instance;
        private static readonly Lock _lock = new();
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");

        private ConfigData _config;

        // Private constructor
        private AppReleaseConfig()
        {
            _config = LoadConfig();
        }

        // Java-style GetInstance method
        public static AppReleaseConfig GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new AppReleaseConfig();
                }
            }
            return _instance;
        }
        public string AppVersion { get; set; } = "1.3";

        // Properties
        public ConfigData Config => _config;

        public string Url
        {
            get
            {
                var prefs = UserPreferences.Load();
                if (prefs.AllowCustomHomepage && Uri.IsWellFormedUriString(prefs.HomepageUrl, UriKind.Absolute))
                    return prefs.HomepageUrl;

                return _config.Url;
            }
        }

        public string Title => _config.Title;
        public string AppIntro => _config.AppIntro;
        public string IconPath => _config.IconPath;
        public string TopbarColor => _config.TopbarColor;
        public string AccentColor => _config.AccentColor;
        public string ArabicName => _config.ArabicName;

        public string ApplyTo(Window window)
        {
            window.Title = _config.Title;
            SetIcon(window);
            return TopbarColor;
        }

        private void SetIcon(Window window)
        {
            try
            {
                string iconFullPath = Path.Combine(AppContext.BaseDirectory, _config.IconPath);
                if (File.Exists(iconFullPath))
                {
                    window.Icon = new BitmapImage(new Uri(iconFullPath));
                }
            }
            catch
            {
                // Silently ignore icon issues
            }
        }

        private ConfigData LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                var defaultConfig = new ConfigData();
                try
                {
                    string defaultJson = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(ConfigPath, defaultJson);
                }
                catch { }
                return defaultConfig;
            }

            try
            {
                string json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
            }
            catch
            {
                return new ConfigData(); // fallback
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch {
                MessageBox.Show("Failed to update portal settings.", "Portal Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Reload()
        {
            _config = LoadConfig();
        }

        // Inner data class
        public class ConfigData
        {
            public string Url { get; set; } = "http://swvmasaha/FarmsPortal_test";
            public string Title { get; set; } = "Desktop Portal";
            public string IconPath { get; set; } = "icon.ico";
            public string TopbarColor { get; set; } = "#dddddd";
            public string  ArabicName { get; set; } = "";
            public string AccentColor { get; set; } = "#99dddddd";
            public string AppIntro { get; internal set; } = "This application is designed to provide a user-friendly interface for webapps";
        }
    }
}
