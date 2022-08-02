using System.Windows;
using System.Windows.Input;

namespace AppLauncher.Infrastructure.Helpers
{
    public static class WindowPositionHelper
    {
        public static void SetMainWindowPosition()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null) return;


            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            var left = (screenWidth - mainWindow.Width) / 2;
            var top = (screenHeight - mainWindow.Height) - 50;

            mainWindow.Left = left;
            mainWindow.Top = top;
        }
    }
}
