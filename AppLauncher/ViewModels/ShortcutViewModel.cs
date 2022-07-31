using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Views;
using GongSolutions.Wpf.DragDrop;
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


        #region ShortcutPath : string - Путь для запуска

        /// <summary>Путь для запуска</summary>
        private string _ShortcutPath;

        /// <summary>Путь для запуска</summary>
        public string ShortcutPath
        {
            get => _ShortcutPath;
            set => Set(ref _ShortcutPath, value);
        }

        #endregion


        #region Image : ImageSource - Изображение ярлыка

        /// <summary>Изображение ярлыка</summary>
        public ImageSource Image
        {
            get
            {
                var iconFromShortcut = App.ShortcutService.GetIconFromShortcut(ShortcutPath);
                HasImage = iconFromShortcut != null;
                return iconFromShortcut;
            }
        }

        #endregion

   
        #region HasImage : bool - Есть ли изображение

        /// <summary>Есть ли изображение</summary>
        private bool _HasImage;

        /// <summary>Есть ли изображение</summary>
        public bool HasImage
        {
            get => _HasImage;
            set => Set(ref _HasImage, value);
        }

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
            App.ShortcutService.StartProcess(ShortcutPath);
        }

        #endregion


        #region Command RenameCommand - Переименовать ярлык

        /// <summary>Переименовать ярлык</summary>
        private Command _RenameCommand;

        /// <summary>Переименовать ярлык</summary>
        public Command RenameCommand => _RenameCommand
            ??= new Command(OnRenameCommandExecuted, CanRenameCommandExecute, "Переименовать ярлык");

        /// <summary>Проверка возможности выполнения - Переименовать ярлык</summary>
        private bool CanRenameCommandExecute() => true;

        /// <summary>Логика выполнения - Переименовать ярлык</summary>
        private void OnRenameCommandExecuted()
        {
            var vm = new InputBoxWindowViewModel()
            {
                Caption = $"Введите новое имя для ярлыка {Name}:",
                Result = Name
            };
            var wnd = new InputBoxWindow()
            {
                Owner = App.ActiveWindow,
                DataContext = vm,
            };
            if (wnd.ShowDialog() != true) return;

            Name = vm.Result;

            App.DataManager.SaveData();
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
            App.ShortcutService.DeleteShortcut(ShortcutPath);
            cell.Remove(this);
            App.DataManager.SaveData();
        }

        #endregion


        #region Command MakeBigShortcutCommand - Сделать большим

        /// <summary>Сделать большим</summary>
        private Command _MakeBigShortcutCommand;

        /// <summary>Сделать большим</summary>
        public Command MakeBigShortcutCommand => _MakeBigShortcutCommand
            ??= new Command(OnMakeBigShortcutCommandExecuted, CanMakeBigShortcutCommandExecute, "Сделать большим");

        /// <summary>Проверка возможности выполнения - Сделать большим</summary>
        private bool CanMakeBigShortcutCommandExecute() => !Equals(FindCell().BigShortcutViewModel, this);

        /// <summary>Логика выполнения - Сделать большим</summary>
        private void OnMakeBigShortcutCommandExecuted()
        {
            var cell = FindCell();
            var others = cell.GetAllShortcuts();

            others.Remove(this);
            var group = App.MainWindowViewModel.Groups.First(g => g.Id == cell.GroupId);
            group.AddShortcuts(others.ToArray());

            others.ForEach(sh => cell.Remove(sh));
            cell.Remove(this);
            cell.BigShortcutViewModel = this;
            App.DataManager.SaveData();
        }

        #endregion


        #region Command MakeLittleCommand - Сделать маленьким

        /// <summary>Сделать маленьким</summary>
        private Command _MakeLittleCommand;

        /// <summary>Сделать маленьким</summary>
        public Command MakeLittleCommand => _MakeLittleCommand
            ??= new Command(OnMakeLittleCommandExecuted, CanMakeLittleCommandExecute, "Сделать маленьким");

        /// <summary>Проверка возможности выполнения - Сделать маленьким</summary>
        private bool CanMakeLittleCommandExecute() => Equals(FindCell().BigShortcutViewModel, this);

        /// <summary>Логика выполнения - Сделать маленьким</summary>
        private void OnMakeLittleCommandExecuted()
        {
            var cell = FindCell();
            cell.BigShortcutViewModel = null;
            cell.ShortcutViewModel1 = this;
            App.DataManager.SaveData();
        }

        #endregion


        #region Command GoToFileCommand - Перейти в расположение файла

        /// <summary>Перейти в расположение файла</summary>
        private Command _GoToFileCommand;

        /// <summary>Перейти в расположение файла</summary>
        public Command GoToFileCommand => _GoToFileCommand
            ??= new Command(OnGoToFileCommandExecuted, CanGoToFileCommandExecute, "Перейти в расположение файла");

        /// <summary>Проверка возможности выполнения - Перейти в расположение файла</summary>
        private bool CanGoToFileCommandExecute() => true;

        /// <summary>Логика выполнения - Перейти в расположение файла</summary>
        private void OnGoToFileCommandExecuted()
        {
            var path = App.ShortcutService.GetFilePath(ShortcutPath);

            var directoryName = Path.GetDirectoryName(path);
            if (directoryName == null) return;

            var argument = "/select, \"" + path + "\"";

            Process.Start(new ProcessStartInfo("explorer.exe", argument)
            {
                UseShellExecute = true
            });
        }

        #endregion

        #endregion


        /// <summary> Найти ячейку, содержащую этот ярлык </summary>
        public ShortcutCellViewModel FindCell()
        {
            var groups = App.MainWindowViewModel.Groups;
            foreach (var group in groups)
            {
                var find = group.ShortcutCells.FirstOrDefault(sc => sc.GetAllShortcuts().Contains(this));
                if (find != null)
                    return find;
            }

            throw new ArgumentOutOfRangeException(nameof(Name), "Ячейка не найдена");
        }


    }
}
