using System.Windows;
using AppLauncher.Services;
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

        public static LinkService LinkService { get; } = new (new ShortcutCreator());
    }
}
