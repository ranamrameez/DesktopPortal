using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DesktopPortal.Helpers
{
    class GeneralHelpers
    {
        public static void LoadIcon(Image AppIcon)
        {
            try
            {
                string iconFullPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");
                var iconUri = new Uri($"file:///{iconFullPath.Replace("\\", "/")}");

                if (File.Exists(iconFullPath))
                {
                    AppIcon.Source = new BitmapImage(iconUri);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading icon: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
