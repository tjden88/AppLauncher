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
    public class DataManager
    {
        private readonly LinkService _LinkService;
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Settings.json");

        public DataManager(LinkService LinkService)
        {
            _LinkService = LinkService;
        }

        private class AppData
        {
            public List<AppLink> Links { get; set; } = new();
            public List<Group> Groups { get; set; } = new();
        }

        private AppData _Data;

        private AppData Data => _Data ??= LoadData();

        private AppData LoadData()
        {
            var data = DataSerializer.LoadFromFile<AppData>(_SettingsFileName);
            return data ?? new AppData();
        }

        private void SaveData() => DataSerializer.SaveToFile(Data, _SettingsFileName);



        #region Public

        /// <summary>
        /// Загрузить группы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Group> LoadGroups() => Data.Groups.OrderBy(g => g.Id);

        /// <summary>
        /// Добавить группу
        /// </summary>
        /// <param name="Name">Имя группы</param>
        /// <returns></returns>
        public Group AddGroup(string Name)
        {
            var grName = string.IsNullOrEmpty(Name)
                ? "Новая группа"
                : Name;
            var group = new Group()
            {
                Id = Data.Groups
                    .Select(g => g.Id)
                    .DefaultIfEmpty()
                    .Max() + 1,
                Name = grName,
            };
            Data.Groups.Add(group);
            SaveData();
            return group;
        }

        /// <summary>
        /// Загрузить ярлыки группы
        /// </summary>
        /// <param name="GroupId">Id группы</param>
        public IEnumerable<AppLink> LoadGroupLinks(int GroupId)
        {
            var links = Data.Links
                .Where(l => l.GroupId == GroupId)
                .ToArray();

            var existLinks = links
                .Where(l => File.Exists(l.Path))
                .ToArray();

            var dist = links.Except(existLinks).ToArray();
            if (dist.Any()) // Некоторые ярлыки не найдены
            {
                foreach (var appLink in dist) Data.Links.Remove(appLink);
                SaveData();
            }
            return existLinks;
        }


        /// <summary>
        /// Добавить ярлык в группу
        /// </summary>
        /// <param name="Path">Путь к источнику</param>
        /// <param name="GroupId">Id группы</param>
        public AppLink AddAppLink(string Path, int GroupId)
        {
            var link = _LinkService.CreateLink(Path);
            link.GroupId = GroupId;
            Data.Links.Add(link);
            SaveData();
            return link;
        }


        #endregion
    }
}
