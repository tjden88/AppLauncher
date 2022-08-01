using System;
using System.IO;
using System.Windows;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;
using WPR.Tools;

namespace AppLauncher.ViewModels
{
    public class SettingsWindowViewModel : ViewModel
    {
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Settings.json");

        private class AppSettings
        {
            public bool IsTopMost { get; set; }

            public bool AutoHide { get; set; }

            public int WindowWidth { get; set; }

            public int WindowHeight { get; set; }
        }

        private AppSettings _Settings;

        public SettingsWindowViewModel()
        {
            LoadData();
        }


        private void LoadData()
        {
            _Settings = DataSerializer.LoadFromFile<AppSettings>(_SettingsFileName) ?? new AppSettings();
            WindowWidth = _Settings.WindowWidth;
            WindowHeight = _Settings.WindowHeight;
            AutoHide = _Settings.AutoHide;
            IsTopMost = _Settings.IsTopMost;
        }

        public void SaveData()
        {
            var sett = new AppSettings
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                AutoHide = AutoHide,
                IsTopMost = IsTopMost,
            };
            DataSerializer.SaveToFile(sett, _SettingsFileName);
        }

        #region WindowWidth : int - Ширина окна

        /// <summary>Ширина окна</summary>
        private int _WindowWidth;

        /// <summary>Ширина окна</summary>
        public int WindowWidth
        {
            get => _WindowWidth;
            set => Set(ref _WindowWidth, value);
        }

        #endregion


        #region WindowHeight : int - Высота окна

        /// <summary>Высота окна</summary>
        private int _WindowHeight;

        /// <summary>Высота окна</summary>
        public int WindowHeight
        {
            get => _WindowHeight;
            set => Set(ref _WindowHeight, value);
        }

        #endregion


        #region IsTopMost : bool - Поверх всех окон

        /// <summary>Поверх всех окон</summary>
        private bool _IsTopMost;

        /// <summary>Поверх всех окон</summary>
        public bool IsTopMost
        {
            get => _IsTopMost;
            set => Set(ref _IsTopMost, value);
        }

        #endregion


        #region AutoHide : bool - Скрывать автоматически

        /// <summary>Скрывать автоматически</summary>
        private bool _AutoHide = true;

        /// <summary>Скрывать автоматически</summary>
        public bool AutoHide
        {
            get => _AutoHide;
            set => Set(ref _AutoHide, value);
        }

        #endregion


        #region StartWithWindows : bool - Запускаться с системой

        /// <summary>Запускаться с системой</summary>
        private bool _StartWithWindows;

        /// <summary>Запускаться с системой</summary>
        public bool StartWithWindows
        {
            get => _StartWithWindows;
            set => Set(ref _StartWithWindows, value);
        }

        #endregion


        #region Command SaveSettingsCommand - Сохранить настройки

        /// <summary>Сохранить настройки</summary>
        private Command _SaveSettingsCommand;

        /// <summary>Сохранить настройки</summary>
        public Command SaveSettingsCommand => _SaveSettingsCommand
            ??= new Command(OnSaveSettingsCommandExecuted, CanSaveSettingsCommandExecute, "Сохранить настройки");

        /// <summary>Проверка возможности выполнения - Сохранить настройки</summary>
        private bool CanSaveSettingsCommandExecute(object p) => p is Window;

        /// <summary>Логика выполнения - Сохранить настройки</summary>
        private void OnSaveSettingsCommandExecuted(object p)
        {
            SaveData();
            ((Window)p).DialogResult = true;

        }

        #endregion
    }
}
