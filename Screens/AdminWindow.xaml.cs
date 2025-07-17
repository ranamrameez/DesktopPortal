using System.Windows;
using System.Windows.Input;
using DesktopPortal.Helpers;

namespace DesktopPortal.Screens
{
    public partial class AdminWindow : Window
    {
        private UserPreferences? _userPrefs;
        private readonly AppReleaseConfig _appConfig = AppReleaseConfig.GetInstance();

        public AdminWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PortalTitleTextBox.Text = _appConfig.Title;
            PortalUrlTextBox.Text = _appConfig.Url;
            AppVersion.Text = AppReleaseConfig.GetInstance().AppVersion;

            AppIcon.Source = AppAssets.AppIcon;
            AppIcon2.Source = AppAssets.AppIcon;

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
            if ((Keyboard.Modifiers == ModifierKeys.Alt && e.Key == Key.F4))
            {
                Close();
            }
        }

        //private void PickColor_Click(object sender, RoutedEventArgs e)
        //{
        //    var dlg = new System.Windows.Forms.ColorDialog();
        //    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        var color = dlg.Color;
        //        TopbarColorTextBox.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        //    }
        //}

        private void Unlock_Click(object sender, RoutedEventArgs e)
        {
            const string AdminPassword = "911";

            if (PasswordBox.Password == AdminPassword)
            {
                PasswordPanel.Visibility = Visibility.Collapsed;
                SettingsPanel.Visibility = Visibility.Visible;

                _userPrefs = UserPreferences.Load();

                AllowBrowsingCheckBox.IsChecked = _userPrefs.AllowBrowsing;
                ShowUrlBarCheck.IsChecked = _userPrefs.ShowUrlBar;
                AllowCustomHomepageCheckBox.IsChecked = _userPrefs.AllowCustomHomepage;
                HomepageUrlTextBox.Text = _userPrefs.HomepageUrl;

                PortalTitleTextBox.Text = _appConfig.Title;
                PortalUrlTextBox.Text = _appConfig.Url;
                AppVersion.Text = AppReleaseConfig.GetInstance().AppVersion;
                AppIcon2.Source = AppAssets.AppIcon;
            }
            else
            {
                MessageBox.Show("Incorrect password.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Clear();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_userPrefs ==null)
            {
                return;
            }
            // Save user preferences
            _userPrefs.AllowBrowsing = AllowBrowsingCheckBox.IsChecked == true;
            _userPrefs.ShowUrlBar = ShowUrlBarCheck.IsChecked == true;
            _userPrefs.AllowCustomHomepage = AllowCustomHomepageCheckBox.IsChecked == true;
            _userPrefs.HomepageUrl = HomepageUrlTextBox.Text.Trim();
            _userPrefs.Save();

            // Save app config
            string newTitle = PortalTitleTextBox.Text.Trim();
            string newUrl = PortalUrlTextBox.Text.Trim();
            _appConfig.Config.Title = newTitle;
            _appConfig.Config.Url = newUrl;
            _appConfig.Save();

            MessageBox.Show("Settings applied successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
