using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopPortal.Helpers;

public class UserPreferences
{
    [JsonPropertyName("auto_login")]
    public bool AutoLogin { get; set; } = true;

    [JsonPropertyName("skip_exit_prompt")]
    public bool SkipExitPrompt { get; set; } = false;

    [JsonPropertyName("show_url_bar")]
    public bool ShowUrlBar { get; set; } = false;

    [JsonPropertyName("allow_custom_homepage")]
    public bool AllowCustomHomepage { get; set; } = false;

    [JsonPropertyName("allow_browsing")]
    public bool AllowBrowsing { get; set; } = false;

    [JsonPropertyName("homepage_url")]
    public string HomepageUrl { get; set; } = "https://ranamrameez.github.io/Translation-Helper/";

    [JsonPropertyName("app_version")]
    public string AppVersion { get; set; } = AppReleaseConfig.GetInstance().AppVersion;

    /// <summary>
    /// File location: C:\Users\{User}\AppData\Roaming\DesktopPortal\userprefs.json
    /// </summary>
    private static string PrefFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DesktopPortal", "userprefs.json");

    public static UserPreferences Load()
    {
        try
        {
            if (File.Exists(PrefFilePath))
            {
                var json = File.ReadAllText(PrefFilePath);
                var prefs = JsonSerializer.Deserialize<UserPreferences>(json);
                return prefs ?? new UserPreferences();
            }
            else
            {
                // File doesn't exist — create it with defaults
                var defaults = new UserPreferences();
                defaults.Save();
                return defaults;
            }
        }
        catch
        {
            // Fallback on error
            var fallback = new UserPreferences();
            fallback.Save(); // Optional: still create the file
            return fallback;
        }
    }


    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PrefFilePath)!);
            AppVersion = AppReleaseConfig.GetInstance().AppVersion;
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(PrefFilePath, json);
        }
        catch
        {
            // Optional: log error
        }
    }

    public static void ClearAutoLogin()
    {
        var prefs = Load();
        prefs.AutoLogin = false;
        prefs.Save();
    }

    public static void ClearLogin() => SecureStorage.Clear();
}
