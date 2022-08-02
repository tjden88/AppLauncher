using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
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

            public bool HideWhenClosing { get; set; }

            public int WindowWidth { get; set; }

            public int WindowHeight { get; set; }
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
            WindowWidth = _Settings.WindowWidth;
            WindowHeight = _Settings.WindowHeight;
            AutoHide = _Settings.AutoHide;
            HideWhenClosing = _Settings.HideWhenClosing;
            IsTopMost = _Settings.IsTopMost;
        }

        public void SaveData()
        {
            var sett = new AppSettings
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
                AutoHide = AutoHide,
                HideWhenClosing = HideWhenClosing,
                IsTopMost = IsTopMost,
            };
            DataSerializer.SaveToFile(sett, _SettingsFileName);

            if (CheckAutoLaunch() == StartWithWindows) return;

            if(StartWithWindows)
                SetAutoLaunch();
            else
                RemoveAutoLaunch();
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


        #region AutoLaunch

        private readonly string _ExeStartupValue = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName + " -hide";


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
