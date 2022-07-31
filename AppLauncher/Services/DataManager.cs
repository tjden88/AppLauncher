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
        private readonly ShortcutService _ShortcutService;
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Settings.json");

        public DataManager(ShortcutService ShortcutService)
        {
            _ShortcutService = ShortcutService;
        }

        private class AppData
        {
            public List<ShortcutCell> LinksGroups { get; set; } = new();
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

        /// <summary> Переименовать группу </summary>
        public void RenameGroup(string NewName, int GroupId)
        {
            var gr = Data.Groups.First(g => g.Id == GroupId);
            gr.Name = NewName;
            SaveData();
        }
        

        /// <summary>
        /// Загрузить ярлыки группы
        /// </summary>
        /// <param name="GroupId">Id группы</param>
        public IEnumerable<ShortcutCell> LoadGroupLinks(int GroupId)
        {
            var groups = Data.LinksGroups
                .Where(l => l.GroupId == GroupId)
                .ToArray();

            var brokenLinks = new List<Shortcut>();

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

            var loadGroupLinks = LoadGroupLinks(GroupId);
            foreach (var linkGroup in loadGroupLinks)
            {
                linkGroup.GetAllLinks().ForEach(l => File.Delete(l.Path));
                Data.LinksGroups.Remove(linkGroup);
            }



            var removed = Data.Groups.Remove(group);
            if (removed)
                SaveData();
        }

        #endregion


        #region LinkGroups

        public ShortcutCell AddAppLinkGroup(int GroupId)
        {
            var lg = new ShortcutCell
            {
                GroupId = GroupId,
                Id = Data.LinksGroups.Select(l => l.Id).DefaultIfEmpty().Max() + 1,
            };
            Data.LinksGroups.Add(lg);
            SaveData();
            return lg;
        }

        public void UpdateAppLinkGroup(ShortcutCell ShortcutCell)
        {
            var findGroup = Data.LinksGroups.First(g => g.Id == ShortcutCell.Id);
            var index = Data.LinksGroups.IndexOf(findGroup);
            Data.LinksGroups.Remove(findGroup);
            Data.LinksGroups.Insert(index, ShortcutCell);
            SaveData();
        }

        public void DeleteAppLinkGroup(int Id)
        {
            var group = Data.LinksGroups.FirstOrDefault(g => g.Id == Id);
            if (group == null) return;
            var links = group.GetAllLinks();
            links.ForEach(l => File.Delete(l.Path));

            Data.LinksGroups.Remove(group);
            SaveData();
        }

        #endregion

    }
}
