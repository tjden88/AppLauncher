using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppLauncher.Models;
using WPR.Tools;

namespace AppLauncher.Services
{
    /// <summary>
    /// Загрузка и сохранение данных
    /// </summary>
    public  class DataManager
    {
        private readonly LinkService _LinkService;
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Settings.json");

        private class AppData
        {
            public List<AppLink> Links { get; set; } = new();

            public List<Group> Groups { get; set; } = new();
        }


        public DataManager(LinkService LinkService)
        {
            _LinkService = LinkService;
        }


        private AppData _Data;

        private AppData Data => _Data ??= LoadData();

        /// <summary>
        /// Загрузить ярлыки группы
        /// </summary>
        /// <param name="GroupId">Id группы</param>
        public IEnumerable<AppLink> LoadGroupLinks(int GroupId) => Data.Links.Where(l => l.GroupId == GroupId);


        /// <summary>
        /// Добавить ярлык в группу
        /// </summary>
        /// <param name="Path">Путь к источнику</param>
        /// <param name="GroupId">Id группы</param>
        public void AddAppLink(string Path, int GroupId)
        {
            var link = _LinkService.CreateLink(Path);
            link.GroupId = GroupId;
            Data.Links.Add(link);
            SaveData();
        }


        private AppData LoadData()
        {
            var data = DataSerializer.LoadFromFile<AppData>(_SettingsFileName);
            return data ?? new AppData();
        }

        private void SaveData() => DataSerializer.SaveToFile(Data, _SettingsFileName);
    }
}
