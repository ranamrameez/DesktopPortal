using DesktopPortal.Helpers;
using System.Windows;

namespace DesktopPortal.Screens
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            AppReleaseConfig.GetInstance().ApplyTo(this);
            AppIcon.Source = AppAssets.AppIcon;
        }
    }
}
