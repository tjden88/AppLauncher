using System;
using System.IO;
using WPR.MVVM.ViewModels;
using WPR.Tools;

namespace AppLauncher.ViewModels
{
    public class SettingsWindowViewModel : ViewModel
    {
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Settings.json");

        private class AppSettings
        {
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
        }

        private void SaveData()
        {
            var sett = new AppSettings
            {
                WindowWidth = WindowWidth,
                WindowHeight = WindowHeight,
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

    }
}
