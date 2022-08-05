using System;
using System.Windows;
using AppLauncher.ViewModels;

namespace AppLauncher.Infrastructure.Helpers
{
    public static class WindowPositionHelper
    {
        public static void SetMainWindowPosition(WindowsStartPosition StartPosition)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null) return;


            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            double left;
            double top;

            switch (StartPosition)
            {
                case WindowsStartPosition.Center:
                    left = (screenWidth - mainWindow.Width) / 2;
                    top = (screenHeight - mainWindow.Height) / 2;
                    break;
                case WindowsStartPosition.BottomCenter:
                    left = (screenWidth - mainWindow.Width) / 2;
                    top = (screenHeight - mainWindow.Height) - 10;
                    break;
                case WindowsStartPosition.BottomLeft:
                    left = 10;
                    top = (screenHeight - mainWindow.Height) - 10;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(StartPosition), StartPosition, null);
            }

            mainWindow.Left = left;
            mainWindow.Top = top;
        }

        public static void SetMainWindowSize()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null) return;

            mainWindow.Height = App.SettingsWindowViewModel.WindowHeight;
        }
    }
}
