using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using AppLauncher.Views;
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
                var iconFromShortcut = App.ShortcutManager.GetIconFromShortcut(ShortcutPath, IsDefineIconFromShortcut);
                HasImage = iconFromShortcut != null;
                return iconFromShortcut;
            }
        }

        #endregion


        #region IsDefineIconFromShortcut : bool - Сначала определить иконку из свойств ярлыка

        /// <summary>Сначала определить иконку из свойств ярлыка</summary>
        private bool _IsDefineIconFromShortcut;

        /// <summary>Сначала определить иконку из свойств ярлыка</summary>
        public bool IsDefineIconFromShortcut
        {
            get => _IsDefineIconFromShortcut;
            set => IfSet(ref _IsDefineIconFromShortcut, value)
                .CallPropertyChanged(nameof(Image))
                .CallPropertyChanged(nameof(IsDefineIconDefault));
        }

        #endregion


        #region IsDefineIconDefault : bool - Источник значка - по умолчанию

        /// <summary>Источник значка - по умолчанию</summary>
        public bool IsDefineIconDefault
        {
            get => !_IsDefineIconFromShortcut;
            set => IsDefineIconFromShortcut = !value;
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
            App.ShortcutManager.StartProcess(ShortcutPath);

            if (App.SettingsWindowViewModel.AutoHide &&
                Keyboard.Modifiers != ModifierKeys.Control &&
                !App.MainWindowViewModel.KeepOpen)

                App.MainWindowViewModel.IsHidden = true;

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
            App.ShortcutManager.DeleteShortcut(ShortcutPath);
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
        private bool CanGoToFileCommandExecute() => PathToDirectory() != null;

        /// <summary>Логика выполнения - Перейти в расположение файла</summary>
        private void OnGoToFileCommandExecuted()
        {
            var path = PathToDirectory();

            var argument = "/select, \"" + App.ShortcutManager.GetFilePath(ShortcutPath) + "\"";

            Process.Start(new ProcessStartInfo("explorer.exe", argument)
            {
                UseShellExecute = true
            });
        }

        #endregion


        #region Command GoToShortcutCommand - Перейти к ярлыку

        /// <summary>Перейти к ярлыку</summary>
        private Command _GoToShortcutCommand;

        /// <summary>Перейти к ярлыку</summary>
        public Command GoToShortcutCommand => _GoToShortcutCommand
            ??= new Command(OnGoToShortcutCommandExecuted, CanGoToShortcutCommandExecute, "Перейти к ярлыку");

        /// <summary>Проверка возможности выполнения - Перейти к ярлыку</summary>
        private bool CanGoToShortcutCommandExecute() => true;

        /// <summary>Логика выполнения - Перейти к ярлыку</summary>
        private void OnGoToShortcutCommandExecuted()
        {
            var shortcutManager = App.ShortcutManager;

            var path = shortcutManager.GetShortcutFullPath(ShortcutPath);

            var argument = "/select, \"" + path + "\"";

            Process.Start(new ProcessStartInfo("explorer.exe", argument)
            {
                UseShellExecute = true
            });
        }

        #endregion


        #region Command RefreshIconCommand - Обновить значок

        /// <summary>Обновить значок</summary>
        private Command _RefreshIconCommand;

        /// <summary>Обновить значок</summary>
        public Command RefreshIconCommand => _RefreshIconCommand
            ??= new Command(OnRefreshIconCommandExecuted, CanRefreshIconCommandExecute, "Обновить значок");

        /// <summary>Проверка возможности выполнения - Обновить значок</summary>
        private bool CanRefreshIconCommandExecute() => true;

        /// <summary>Логика выполнения - Обновить значок</summary>
        private void OnRefreshIconCommandExecuted() => OnPropertyChanged(nameof(Image));

        #endregion


        #region Command SetDefautIconSourceCommand - Установить источник иконки по умолчанию

        /// <summary>Установить источник иконки по умолчанию</summary>
        private Command _SetDefautIconSourceCommand;

        /// <summary>Установить источник иконки по умолчанию</summary>
        public Command SetDefautIconSourceCommand => _SetDefautIconSourceCommand
            ??= new Command(OnSetDefautIconSourceCommandExecuted, CanSetDefautIconSourceCommandExecute, "Установить источник иконки по умолчанию");

        /// <summary>Проверка возможности выполнения - Установить источник иконки по умолчанию</summary>
        private bool CanSetDefautIconSourceCommandExecute() => !IsDefineIconDefault;

        /// <summary>Логика выполнения - Установить источник иконки по умолчанию</summary>
        private void OnSetDefautIconSourceCommandExecuted()
        {
            IsDefineIconDefault = true;
            App.DataManager.SaveData();
        }

        #endregion


        #region Command SetShortcutIconSourceCommand - Установить источником иконки данные ярлыка

        /// <summary>Установить источником иконки данные ярлыка</summary>
        private Command _SetShortcutIconSourceCommand;

        /// <summary>Установить источником иконки данные ярлыка</summary>
        public Command SetShortcutIconSourceCommand => _SetShortcutIconSourceCommand
            ??= new Command(OnSetShortcutIconSourceCommandExecuted, CanSetShortcutIconSourceCommandExecute, "Установить источником иконки данные ярлыка");

        /// <summary>Проверка возможности выполнения - Установить источником иконки данные ярлыка</summary>
        private bool CanSetShortcutIconSourceCommandExecute() => !IsDefineIconFromShortcut;

        /// <summary>Логика выполнения - Установить источником иконки данные ярлыка</summary>
        private void OnSetShortcutIconSourceCommandExecuted()
        {
            IsDefineIconFromShortcut = true;
            App.DataManager.SaveData();
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


        private string PathToDirectory()
        {
            var path = App.ShortcutManager.GetFilePath(ShortcutPath);

            var directoryName = Directory.Exists(path) ? path : Path.GetDirectoryName(path);
            return directoryName;
        }

    }
}
