using System.Windows;
using System.Windows.Input;
using DesktopPortal.Helpers;

namespace DesktopPortal.Screens;

public partial class SettingsWindow : Window
{
    private UserPreferences _prefs;
    private AppReleaseConfig _appReleaseConfig = AppReleaseConfig.GetInstance();
    public SettingsWindow()
    {
        //🔒
        InitializeComponent();
        _prefs = UserPreferences.Load();
        AutoLoginCheck.IsChecked = _prefs.AutoLogin;
        SkipExitCheck.IsChecked = _prefs.SkipExitPrompt;
        ShowUrlBarCheck.IsChecked = _prefs.ShowUrlBar;
        if(_prefs.AllowBrowsing)
        {
            ShowUrlBarCheck.Visibility = Visibility.Visible;            
        }
        else
        {
            ShowUrlBarCheck.Visibility = Visibility.Collapsed;
        }
        AppName.Text = _appReleaseConfig.Title;
        AppIntro.Text = _appReleaseConfig.AppIntro;
        HomepageBox.Text = _prefs.HomepageUrl;
        HomepageBox.IsEnabled = _prefs.AllowCustomHomepage;
        AppVersion.Text = _appReleaseConfig.AppVersion;
        AppIcon.Source = AppAssets.AppIcon;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _prefs.AutoLogin = AutoLoginCheck.IsChecked == true;
        _prefs.SkipExitPrompt = SkipExitCheck.IsChecked == true;
        _prefs.ShowUrlBar = ShowUrlBarCheck.IsChecked == true;
        _prefs.HomepageUrl = HomepageBox.Text.Trim();
        _prefs.Save();
        //MessageBox.Show("Settings saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    private void ClearLoginInfo_Click(object sender, RoutedEventArgs e)
    {
        SecureStorage.Clear(); // Clears saved username/password
        UserPreferences.ClearAutoLogin(); // Clears auto login flag
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.Close();
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.Close();
        }
        if ((Keyboard.Modifiers == ModifierKeys.Alt && e.Key == Key.F4)) Close();        
    }

}
