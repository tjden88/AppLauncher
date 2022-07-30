using System.Windows;
using AppLauncher.ViewModels;

namespace AppLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary> Вьюмодель главного окна </summary>
        public static MainWindowViewModel MainWindowViewModel { get; } = new();
    }
}
