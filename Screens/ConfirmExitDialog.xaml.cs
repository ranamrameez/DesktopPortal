using DesktopPortal.Helpers;
using System.Windows;
using System.Windows.Input;

namespace DesktopPortal.Screens
{
    public partial class ConfirmExitDialog : Window
    {
        public bool DontAskAgain => DontAskAgainCheckBox.IsChecked == true;

        public ConfirmExitDialog()
        {
            InitializeComponent();
            AppReleaseConfig.GetInstance().ApplyTo(this);

        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            DialogResult = false;
            this.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Cancel();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Cancel();
            if ((Keyboard.Modifiers == ModifierKeys.Alt && e.Key == Key.F4)) Cancel();
            
        }
    }
}
