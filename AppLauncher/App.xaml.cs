using System.Linq;
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

        public static SettingsWindowViewModel SettingsWindowViewModel { get; } = new();

        public static ShortcutManager ShortcutManager { get; } = new (new IconBuilder(), new ShortcutBuilder());

        public static DataManager DataManager { get; } = new();

        public static Window ActiveWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);

        protected override void OnExit(ExitEventArgs e)
        {
            ShortcutManager.CleanNotUsedShortcuts();
            base.OnExit(e);
        }

    }
}
