using System;
using System.Linq;
using System.Windows;
using AppLauncher.ViewModels;
using GongSolutions.Wpf.DragDrop;

namespace AppLauncher.Infrastructure.Helpers
{
    public static class DragDropHelper
    {

        [Flags]
        public enum DropType
        {
            None = 0,
            Files = 1,
            ShortcutViewModel = 2,
            ShortcutCellViewModel = 4,
            GroupViewModel = 8,
        }

        /// <summary>
        /// Содержит ссылки на перенесённые мышккой объекты
        /// </summary>
        public class DroppedObjects
        {
            public ShortcutViewModel[] Shortcuts { get; set; }
            public ShortcutCellViewModel ShortcutCell { get; set; }
            public GroupViewModel Group { get; set; }

        }

        /// <summary>
        /// Перетаскивание мышью на объектом
        /// </summary>
        /// <param name="dropInfo">Инфо</param>
        /// <param name="AcceptedTypes">Разрешённые типы данных</param>
        public static void DragOver(IDropInfo dropInfo, DropType AcceptedTypes)
        {
            if (AcceptedTypes.HasFlag(DropType.None)) return;

            var sourceItem = dropInfo.Data;

            var accept = sourceItem switch
            {
                ShortcutViewModel => AcceptedTypes.HasFlag(DropType.ShortcutViewModel),
                ShortcutCellViewModel => AcceptedTypes.HasFlag(DropType.ShortcutCellViewModel),
                GroupViewModel => AcceptedTypes.HasFlag(DropType.GroupViewModel),
                DataObject dataObject when dataObject.GetData(DataFormats.FileDrop) is string[] =>
                    AcceptedTypes.HasFlag(DropType.Files),
                _ => false
            };


            if (accept)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }
            dropInfo.Effects = DragDropEffects.None;
        }

        /// <summary> Подготовить перенесённые мышкой объекты </summary>
        public static DroppedObjects PerformDrop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data;
            var droppedObjects = new DroppedObjects();

            if (sourceItem is ShortcutViewModel shortcutViewModel)
            {
                shortcutViewModel.FindCell().Remove(shortcutViewModel);
                droppedObjects.Shortcuts = new[] { shortcutViewModel };
            }

            if (dropInfo.Data is GroupViewModel groupViewModel)
            {
                App.MainWindowViewModel.Groups.Remove(groupViewModel);
                droppedObjects.Group = groupViewModel;
            }

            if (sourceItem is ShortcutCellViewModel shortcutCellViewModel)
            {
                var group = App.MainWindowViewModel.Groups.First(g => g.Id == shortcutCellViewModel.GroupId);
                group.ShortcutCells.Remove(shortcutCellViewModel);
                droppedObjects.ShortcutCell = shortcutCellViewModel;
            }

            if (sourceItem is DataObject dataObject && dataObject.GetData(DataFormats.FileDrop) is string[] strArray)
            {
                var shortcutService = App.ShortcutService;

                var viewModels = strArray
                    .Select(shortcutService.CreateShortcut)
                    .Select(sh => sh.ToViewModel());

                droppedObjects.Shortcuts = viewModels.ToArray();
            }

            return droppedObjects;
        }
    }
}
