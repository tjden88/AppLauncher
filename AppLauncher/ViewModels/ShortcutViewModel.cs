﻿using System;
using System.Linq;
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
        public ImageSource Image => App.ShortcutService.GetIconFromShortcut(ShortcutPath);

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
        private bool CanMakeBigShortcutCommandExecute() => !Equals(FindCell().BigLinkViewModel, this);

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
            cell.BigLinkViewModel = this;
            App.DataManager.SaveData();
        }

        #endregion

        #endregion

        private ShortcutCellViewModel FindCell()
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
