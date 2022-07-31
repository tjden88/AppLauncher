using System;
using System.Linq;
using System.Windows;
using AppLauncher.ViewModels;
using GongSolutions.Wpf.DragDrop;

namespace AppLauncher.Infrastructure.Helpers
{
    public static class DragDropHelper
    {
        public static void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data;

            if (sourceItem is DataObject dataObject && dataObject.GetData(DataFormats.FileDrop) is string[])
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }

            dropInfo.Effects = DragDropEffects.None;
        }


        public static ShortcutViewModel[] Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data;

            if (sourceItem is not DataObject dataObject ||
                dataObject.GetData(DataFormats.FileDrop) is not string[] strArray) return Array.Empty<ShortcutViewModel>();

            var shortcutService = App.ShortcutService;

            var vm = strArray
                .Select(shortcutService.CreateShortcut)
                .Select(sh => sh.ToViewModel());

            return vm.ToArray();
        }
    }
}
