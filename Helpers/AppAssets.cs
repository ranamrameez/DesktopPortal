using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DesktopPortal.Helpers
{
    public static class AppAssets
    {
        private static BitmapImage? _appIcon;

        public static BitmapImage? AppIcon
        {
            get
            {
                _appIcon ??= LoadIcon();
                return _appIcon;
            }
        }


        private static BitmapImage LoadIcon()
        {
            try
            {
                string iconFullPath = Path.Combine(AppContext.BaseDirectory, "icon.ico");

                if (!File.Exists(iconFullPath))
                    throw new FileNotFoundException($"Icon not found: {iconFullPath}");

                var bitmap = new BitmapImage();
                using (var stream = File.OpenRead(iconFullPath))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading icon: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null!;
            }
        }




    }

}
