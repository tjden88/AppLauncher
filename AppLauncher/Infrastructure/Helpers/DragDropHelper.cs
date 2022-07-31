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

            switch (sourceItem)
            {
                case ShortcutViewModel:
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                    break;

                case DataObject dataObject when dataObject.GetData(DataFormats.FileDrop) is string[]:
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Copy;
                    break;

                default:

                    dropInfo.Effects = DragDropEffects.None;
                    break;
            }
        }


        public static ShortcutViewModel[] Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data;

            if (sourceItem is ShortcutViewModel vm)
            {
                vm.FindCell().Remove(vm);
                return new[] { vm };
            }

            if (sourceItem is not DataObject dataObject ||
                dataObject.GetData(DataFormats.FileDrop) is not string[] strArray) return Array.Empty<ShortcutViewModel>();

            var shortcutService = App.ShortcutService;

            var viewModels = strArray
                .Select(shortcutService.CreateShortcut)
                .Select(sh => sh.ToViewModel());

            return viewModels.ToArray();
        }
    }
}
