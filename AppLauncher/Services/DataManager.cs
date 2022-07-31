using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Models;
using WPR.Tools;

namespace AppLauncher.Services
{
    /// <summary>
    /// Загрузка и сохранение данных
    /// </summary>
    public class DataManager
    {
        private readonly string _SettingsFileName = Path.Combine(Environment.CurrentDirectory, "Config.json");

        public DataManager()
        {
        }

        private class AppData
        {
            public List<ShortcutCell> ShortcutCells { get; set; } = new();
            public List<Group> Groups { get; set; } = new();
        }

        private AppData LoadData()
        {
            var data = DataSerializer.LoadFromFile<AppData>(_SettingsFileName);
            return data ?? new AppData();
        }


        private int _NextGroupId;
        private int _NextCellId;

        /// <summary> Получить ID для следующей группы </summary>
        public int GetNextGroupId() => _NextGroupId ++;


        /// <summary> Получить ID для следующей ячейки </summary>
        public int GetNextCellId() => _NextCellId ++;


        /// <summary>
        /// Загрузить данные с диска
        /// </summary>
        /// <returns>Список групп и вложенные в него списки</returns>
        public IEnumerable<Group> LoadGroupsData()
        {
            var data = LoadData();

            foreach (var group in data.Groups)
                group.Cells = data.ShortcutCells
                    .Where(c => c.GroupId == group.Id)
                    .ToList();

            _NextGroupId = data.Groups.Select(g=> g.Id).DefaultIfEmpty().Max() + 1;
            _NextCellId = data.ShortcutCells.Select(g=> g.Id).DefaultIfEmpty().Max() + 1;

            return data.Groups.OrderBy(g => g.Id);
        }

        /// <summary>Обновить и сохранить данные </summary>
        public void SaveData()
        {
            var data = new AppData();

            var groups = App.MainWindowViewModel.Groups;
            foreach (var group in groups)
            {
                var cells = group.ShortcutCells;
                var cellsModels = cells.Select(c => c.ToModel());
                data.ShortcutCells = cellsModels.ToList();
            }

            data.Groups = groups.Select(g => g.ToModel()).ToList();

            DataSerializer.SaveToFile(data, _SettingsFileName);
        }

    }
}
