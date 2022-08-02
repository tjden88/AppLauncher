using System.Windows;

namespace AppLauncher.Infrastructure.Helpers
{
    public static class WindowPositionHelper
    {
        public static void SetMainWindowPosition()
        {
            var mainWindow = Application.Current.MainWindow;
            mainWindow.Left = 0;
            mainWindow.Top = 0;
        }
    }
}
