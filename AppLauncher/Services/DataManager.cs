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
            public List<AppLinkGroup> LinksGroups { get; set; } = new();
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



        #region Groups

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
        public IEnumerable<AppLinkGroup> LoadGroupLinks(int GroupId)
        {
            var groups = Data.LinksGroups
                .Where(l => l.GroupId == GroupId)
                .ToArray();

            var brokenLinks = new List<AppLink>();

            foreach (var linkGroup in groups)
            {
                var linksInGroup = linkGroup.GetAllLinks();
                brokenLinks.AddRange(linksInGroup.Where(l => !File.Exists(l.Path)));
            }


            if (brokenLinks.Any()) // Некоторые ярлыки не найдены
            {
                foreach (var appLink in brokenLinks) 
                    Data.LinksGroups.ForEach(g => g.Remove(appLink));

                SaveData();
            }
            return groups;
        }


        public void DeleteGroup(int GroupId)
        {
            var group = Data.Groups.FirstOrDefault(g => g.Id == GroupId);
            if (group == null) return;

            foreach (var linkGroup in LoadGroupLinks(GroupId))
                linkGroup.GetAllLinks().ForEach(l => File.Delete(l.Path));


            var removed = Data.Groups.Remove(group);
            if (removed)
                SaveData();
        }

        #endregion


        #region LinkGroups

        public AppLinkGroup AddAppLinkGroup(int GroupId)
        {
            var lg = new AppLinkGroup
            {
                GroupId = GroupId,
                Id = Data.LinksGroups.Select(l => l.Id).DefaultIfEmpty().Max() + 1,
            };
            Data.LinksGroups.Add(lg);
            SaveData();
            return lg;
        }

        public void UpdateAppLinkGroup(AppLinkGroup appLinkGroup)
        {
            var findGroup = Data.LinksGroups.First(g => g.Id == appLinkGroup.Id);
            var index = Data.LinksGroups.IndexOf(findGroup);
            Data.LinksGroups.Remove(findGroup);
            Data.LinksGroups.Insert(index, appLinkGroup);
            SaveData();
        }

        #endregion

    }
}
