using System.ComponentModel;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DesktopPortal.Helpers;
using Microsoft.Web.WebView2.Core;

namespace DesktopPortal.Screens;

public partial class MainWindow : Window
{
    private UserPreferences _userPrefs = UserPreferences.Load();
    private bool _isAutologinAttempted = false;
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
        TopBar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppReleaseConfig.GetInstance().ApplyTo(this)));
        ApplyUserPreferences();
    }

    private void AutoLoginToggle_Checked(object sender, RoutedEventArgs e)
    {
        _userPrefs.AutoLogin = AutoLoginToggle.IsChecked == true;
        _userPrefs.Save();
    }

    public async Task InitBrowser()
    {
        try
        {
            string userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DesktopPortal",
                Environment.UserName,
                "WebView2"
            );
            var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);

            await webView.EnsureCoreWebView2Async(env);
            webView.CoreWebView2.Settings.IsZoomControlEnabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize WebView2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        string appUrl = AppReleaseConfig.GetInstance().Url;
        webView.Source = new Uri(appUrl);

        webView.CoreWebView2.HistoryChanged += (s, e) =>
        {
            BackButton.IsEnabled = webView.CoreWebView2.CanGoBack;
        };

        webView.NavigationStarting += (s, e) =>
        {
            LoadingBar.Visibility = Visibility.Visible;
        };

        webView.NavigationCompleted += async (s, e) =>
        {
            LoadingBar.Visibility = Visibility.Collapsed;

            // Remove session flag if user is no longer on login page
            string currentUrl = webView.Source.ToString();
            if (!currentUrl.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                await webView.ExecuteScriptAsync("sessionStorage.removeItem('loginAttempted');");
            }
        };

        EnableAutofill();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Alt)
        {
            if (e.SystemKey == Key.Left) BackButton_Click(null, null);
            else if (e.SystemKey == Key.Right) ForwardButton_Click(null, null);
            else if (e.SystemKey == Key.H) HomeButton_Click(null, null);
        }

        if ((Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R) || e.Key == Key.F5)
        {
            RefreshButton_Click(null, null);
        }

        if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift) && e.Key == Key.A)
        {
            OpenAdminPanel();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => webView.GoBack();
    private void ForwardButton_Click(object sender, RoutedEventArgs e) => webView.GoForward();
    private void RefreshButton_Click(object sender, RoutedEventArgs e) => webView.Reload();
    private void HomeButton_Click(object sender, RoutedEventArgs e) => webView.Source = new Uri(AppReleaseConfig.GetInstance().Url);
    private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = new SettingsWindow
        {
            Owner = this
        };
        settings.ShowDialog();
        ApplyUserPreferences(); // <- Sync AutoLogin toggle, URL bar, etc.
    }

    // Re-read preferences if needed:
    public void ApplyUserPreferences()
    {
        _userPrefs = UserPreferences.Load();
        UrlPanel.Visibility = _userPrefs.ShowUrlBar ? Visibility.Visible : Visibility.Collapsed;
        UrlTextBox.IsReadOnly = !_userPrefs.AllowBrowsing;

        //UrlTextBox.Background = _userPrefs.AllowBrowsing ? Brushes.White : Brushes.LightGray;
        UrlTextBox.ToolTip = _userPrefs.AllowBrowsing
            ? "Current Page URL"
            : "Browsing disabled (edit userprefs.json to enable)";

        AutoLoginToggle.IsChecked = _userPrefs.AutoLogin;
    }

    private string GetWindowsIdentity()
    {
        if (OperatingSystem.IsWindows())
        {
            return WindowsIdentity.GetCurrent().Name;
        }
        else return "";
    }

    private void EnableAutofill()
    {
        webView.CoreWebView2.NavigationCompleted += async (sender, e) =>
        {
            string currentUrl = webView.Source.ToString();
            UrlTextBox.Text = currentUrl;

            if (currentUrl.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                if (_isAutologinAttempted) return;
                var creds = SecureStorage.Load();
                if (creds != null && _userPrefs.AutoLogin)
                {
                    string usernameJs = EscapeForJs(creds.Value.username);
                    string passwordJs = EscapeForJs(creds.Value.password);

                    string fillScript = $@"
                        (function() {{
                            if (sessionStorage.getItem('loginAttempted') === '1') return;

                            setTimeout(() => {{
                                const unameField = document.querySelector('input[id=Input_EmpId]');
                                const pwdField = document.querySelector('input[id=Input_Password]');
                                if (unameField && pwdField) {{
                                    unameField.value = '{usernameJs}';
                                    pwdField.value = '{passwordJs}';
                                    unameField.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                    pwdField.dispatchEvent(new Event('input', {{ bubbles: true }}));
                                    document.querySelector('form')?.submit();
                                }}
                            }}, 300);
                        }})();
                    ";


                    await webView.ExecuteScriptAsync(fillScript);
                    _isAutologinAttempted = true;
                }

                // Capture user input and set 'loginAttempted' flag
                string captureScript = @"
                    (function() {
                        const form = document.querySelector('form');
                        if (form) {
                            form.addEventListener('submit', () => {
                                const uname = document.getElementById('Input_EmpId')?.value || '';
                                const pwd = document.getElementById('Input_Password')?.value || '';
                                sessionStorage.setItem('loginAttempted', '1');
                                chrome.webview.postMessage(JSON.stringify({ username: uname, password: pwd }));
                            });
                        }
                    })();
                ";


                await webView.CoreWebView2.ExecuteScriptAsync(captureScript);
            }
            else
            {
                _isAutologinAttempted = false;
            }
        };

        webView.CoreWebView2.WebMessageReceived += (s, args) =>
        {
            try
            {
                var json = args.WebMessageAsJson;
                var data = System.Text.Json.JsonDocument.Parse(json).RootElement;
                var username = data.GetProperty("username").GetString() ?? "";
                var password = data.GetProperty("password").GetString() ?? "";

                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    SecureStorage.Save(username, password);
                }
            }
            catch
            {
                // Ignore
            }
        };
    }

    private static string EscapeForJs(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"");
    }

    private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (!_userPrefs.AllowBrowsing)
            {
                MessageBox.Show("Manual navigation is disabled.", "Browsing Blocked", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Uri.TryCreate(UrlTextBox.Text, UriKind.Absolute, out Uri? uri))
            {
                webView.Source = uri;
            }
            else
            {
                MessageBox.Show("Invalid URL", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_userPrefs.SkipExitPrompt)
        {
            var dialog = new ConfirmExitDialog
            {
                Owner = this
            };
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                _userPrefs.SkipExitPrompt = dialog.DontAskAgain;
                _userPrefs.Save();
            }
            else if (result != true)
            {
                e.Cancel = true;
            }
        }
        base.OnClosing(e);
    }

    private void OpenAdminPanel()
    {
        var adminWindow = new AdminWindow { Owner = this };
        adminWindow.ShowDialog();

        // Re-apply changes
        ApplyUserPreferences();

        TopBar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppReleaseConfig.GetInstance().ApplyTo(this)));
    }
}
