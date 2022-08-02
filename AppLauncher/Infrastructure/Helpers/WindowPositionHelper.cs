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


            //var screenWidth = SystemParameters.PrimaryScreenWidth;
            //var screenHeight = SystemParameters.PrimaryScreenHeight;


            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;


            var left = (screenWidth - mainWindow.Width) / 2;
            var top = (screenHeight - mainWindow.Height) - 10;

            mainWindow.Left = left;
            mainWindow.Top = top;
        }
    }
}
