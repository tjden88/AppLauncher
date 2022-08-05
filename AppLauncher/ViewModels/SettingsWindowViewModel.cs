using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;
using WPR.Tools;

namespace AppLauncher.ViewModels
{
    public enum WindowsStartPosition
    {
        Center,
        BottomCenter,
        BottomLeft
    }

    public class SettingsWindowViewModel : ViewModel
    {
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Settings.json");

        private const string DonateLink = "https://www.tinkoff.ru/rm/dultsev.denis1/G2APs2254";

        private const string HomeUrl = "https://github.com/tjden88/AppLauncher";

        private const string ReportProblemAddress = "https://github.com/tjden88/AppLauncher/issues";


        #region SettingsData

        private class AppSettings
        {
            public bool IsTopMost { get; set; }

            public bool AutoHide { get; set; }

            public bool HideWhenClosing { get; set; }

            public bool HideWhenLostFocus { get; set; }

            public int GroupWidth { get; set; }

            public int ColumnsCount { get; set; } = 3;

            public int WindowHeight { get; set; } = 750;
        }

        private AppSettings _Settings;

        public SettingsWindowViewModel()
        {
            LoadData();
            StartWithWindows = CheckAutoLaunch();
        }


        private void LoadData()
        {
            _Settings = DataSerializer.LoadFromFile<AppSettings>(_SettingsFileName) ?? new AppSettings();
            WindowHeight = _Settings.WindowHeight;
            AutoHide = _Settings.AutoHide;
            HideWhenClosing = _Settings.HideWhenClosing;
            HideWhenLostFocus = _Settings.HideWhenLostFocus;
            IsTopMost = _Settings.IsTopMost;
        }

        public void SaveData()
        {
            var sett = new AppSettings
            {
                WindowHeight = WindowHeight,
                AutoHide = AutoHide,
                HideWhenClosing = HideWhenClosing,
                HideWhenLostFocus = HideWhenLostFocus,
                IsTopMost = IsTopMost,
            };
            DataSerializer.SaveToFile(sett, _SettingsFileName);

            if (CheckAutoLaunch() == StartWithWindows) return;

            if (StartWithWindows)
                SetAutoLaunch();
            else
                RemoveAutoLaunch();
        }


        #endregion


        /// <summary> Текущая версия приложения </summary>
        public string VersionInfo => "Версия программы: " + App.AppVersion;



        #region BehaviorProps

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


        #region HideWhenClosing : bool - Сворачивать вместо выхода

        /// <summary>Сворачивать вместо выхода</summary>
        private bool _HideWhenClosing = true;

        /// <summary>Сворачивать вместо выхода</summary>
        public bool HideWhenClosing
        {
            get => _HideWhenClosing;
            set => Set(ref _HideWhenClosing, value);
        }

        #endregion


        #region HideWhenLostFocus : bool - Скрывать окно при потере фокуса

        /// <summary>Скрывать окно при потере фокуса</summary>
        private bool _HideWhenLostFocus = true;

        /// <summary>Скрывать окно при потере фокуса</summary>
        public bool HideWhenLostFocus
        {
            get => _HideWhenLostFocus;
            set => Set(ref _HideWhenLostFocus, value);
        }

        #endregion


        #region StartWithWindows : bool - Запускать с системой

        /// <summary>Запускать с системой</summary>
        private bool _StartWithWindows;

        /// <summary>Запускать с системой</summary>
        public bool StartWithWindows
        {
            get => _StartWithWindows;
            set => Set(ref _StartWithWindows, value);
        }

        #endregion

        #endregion

        #region SizeProp


        #region StartPosition : WindowsStartPosition - Позиция при запуске

        /// <summary>Позиция при запуске</summary>
        private WindowsStartPosition _StartPosition;

        /// <summary>Позиция при запуске</summary>
        public WindowsStartPosition StartPosition
        {
            get => _StartPosition;
            set => Set(ref _StartPosition, value);
        }

        #endregion


        #region WindowHeight : int - Высота окна

        /// <summary>Высота окна</summary>
        private int _WindowHeight;

        /// <summary>Высота окна</summary>
        public int WindowHeight
        {
            get => _WindowHeight;
            set
            {
                if(Equals(value, _WindowHeight)) return;

                _WindowHeight = Math.Min(200, Math.Max(value, 1000));

                OnPropertyChanged(nameof(WindowHeight));
            }
        }

        #endregion


        #endregion

        #region Commands

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

        #region Command GoToHomepageCommand - Перейти на страницу проекта

        /// <summary>Перейти на страницу проекта</summary>
        private Command _GoToHomepageCommand;

        /// <summary>Перейти на страницу проекта</summary>
        public Command GoToHomepageCommand => _GoToHomepageCommand
            ??= new Command(OnGoToHomepageCommandExecuted, CanGoToHomepageCommandExecute, "Перейти на страницу проекта");

        /// <summary>Проверка возможности выполнения - Перейти на страницу проекта</summary>
        private bool CanGoToHomepageCommandExecute() => true;

        /// <summary>Логика выполнения - Перейти на страницу проекта</summary>
        private void OnGoToHomepageCommandExecuted() => OpenWebPage(HomeUrl);

        #endregion

        #region Command ReportProblemCommand - Сообщить о проблеме

        /// <summary>Сообщить о проблеме</summary>
        private Command _ReportProblemCommand;

        /// <summary>Сообщить о проблеме</summary>
        public Command ReportProblemCommand => _ReportProblemCommand
            ??= new Command(OnReportProblemCommandExecuted, CanReportProblemCommandExecute, "Сообщить о проблеме");

        /// <summary>Проверка возможности выполнения - Сообщить о проблеме</summary>
        private bool CanReportProblemCommandExecute() => true;

        /// <summary>Логика выполнения - Сообщить о проблеме</summary>
        private void OnReportProblemCommandExecuted() => OpenWebPage(ReportProblemAddress);

        #endregion

        #region Command MakeDonateCommand - Поддержать разработчика

        /// <summary>Поддержать разработчика</summary>
        private Command _MakeDonateCommand;

        /// <summary>Поддержать разработчика</summary>
        public Command MakeDonateCommand => _MakeDonateCommand
            ??= new Command(OnMakeDonateCommandExecuted, CanMakeDonateCommandExecute, "Поддержать разработчика");

        /// <summary>Проверка возможности выполнения - Поддержать разработчика</summary>
        private bool CanMakeDonateCommandExecute() => true;

        /// <summary>Логика выполнения - Поддержать разработчика</summary>
        private void OnMakeDonateCommandExecuted() => OpenWebPage(DonateLink);

        #endregion

        #endregion


        private void OpenWebPage(string Address)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Address,
                UseShellExecute = true
            });
        }


        #region AutoLaunch

        private readonly string _ExeStartupValue = Path.Combine(Environment.CurrentDirectory, "AppLauncher.exe") + " -hide";


        private bool CheckAutoLaunch()
        {
            using var regKey =
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\");
            var appKey = regKey?.GetValue("AppLauncher")?.ToString();
            return appKey == _ExeStartupValue;
        }

        private void SetAutoLaunch()
        {
            using var regKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\");
            if (regKey == null) return;
            regKey.SetValue("AppLauncher", _ExeStartupValue);
            regKey.Close();
        }

        private void RemoveAutoLaunch()
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run\", true);
            if (regKey == null) return;
            regKey.DeleteValue("AppLauncher");
            regKey.Close();
        } 

        #endregion
    }
}
