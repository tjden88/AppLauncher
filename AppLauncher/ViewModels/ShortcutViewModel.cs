using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using AppLauncher.Infrastructure.Helpers;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Модель - представление ярлыка для запуска
    /// </summary>
    public class ShortcutViewModel : ViewModel
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

        #region FilePath : string - Путь для запуска

        /// <summary>Путь для запуска</summary>
        private string _FilePath;

        /// <summary>Путь для запуска</summary>
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        #endregion

        #region Image : ImageSource - Изображение ярлыка

        /// <summary>Изображение ярлыка</summary>
        public ImageSource Image => App.ShortcutService.GetIconFromShortcut(FilePath);

        #endregion


        #region Commands


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
                FileName = FilePath,
                UseShellExecute = true
            });
        }

        #endregion


        #region Command DeleteCommand - Удалить

        /// <summary>Удалить</summary>
        private Command _DeleteCommand;

        /// <summary>Удалить</summary>
        public Command DeleteCommand => _DeleteCommand
            ??= new Command(OnDeleteCommandExecuted, CanDeleteCommandExecute, "Удалить");

        /// <summary>Проверка возможности выполнения - Удалить</summary>
        private bool CanDeleteCommandExecute() => true;

        /// <summary>Логика выполнения - Удалить</summary>
        private void OnDeleteCommandExecuted()
        {
            var cell = FindCell();
            cell.Remove(this);
            App.DataManager.UpdateCell(cell.ToModel());
        }

        #endregion

        #endregion

        private ShortcutCellViewModel FindCell()
        {
            var groups = App.MainWindowViewModel.Groups;
            foreach (var group in groups)
            {
                var find = group.ShortcutCells.FirstOrDefault(sc => sc.GetAllShortcuts().Contains(this));
                if(find != null)
                    return find;
            }

            throw new ArgumentOutOfRangeException(nameof(Name), "Ячейка не найдена");
        }

    }
}
