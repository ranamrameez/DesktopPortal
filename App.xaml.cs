using System.Windows;
using DesktopPortal.Screens;

namespace DesktopPortal
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Show splash (UI thread)
            Screens.SplashScreen splash = new();
            splash.Show();

            // Initialize MainWindow (UI thread)
            MainWindow mainWindow = new();
            mainWindow.Show();

            try
            {
                // Await InitBrowser without blocking UI thread
                await mainWindow.InitBrowser();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Please Restart App. Error initializing browser: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            // Show main window and close splash
            splash.Close();
        }
    }
}
