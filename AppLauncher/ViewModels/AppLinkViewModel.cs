using System.Diagnostics;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Модель - представление ярлыка для запуска
    /// </summary>
    public class AppLinkViewModel : ViewModel
    {

        #region Name : string - Имя ярлыка

        /// <summary>Имя ярлыка</summary>
        private string _Name;

        /// <summary>Имя ярлыка</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion

        #region Path : string - Путь для запуска

        /// <summary>Путь для запуска</summary>
        private string _Path;

        /// <summary>Путь для запуска</summary>
        public string Path
        {
            get => _Path;
            set => Set(ref _Path, value);
        }

        #endregion

        #region Command LaunchCommand - Запуск

        /// <summary>Запуск</summary>
        private Command _LaunchCommand;

        /// <summary>Запуск</summary>
        public Command LaunchCommand => _LaunchCommand
            ??= new Command(OnLaunchCommandExecuted, CanLaunchCommandExecute, "Запуск");

        /// <summary>Проверка возможности выполнения - Запуск</summary>
        private bool CanLaunchCommandExecute() => true;

        /// <summary>Логика выполнения - Запуск</summary>
        private void OnLaunchCommandExecuted()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path,
                UseShellExecute = true
            });
        }

        #endregion
    }
}
