using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppLauncher.Infrastructure.Helpers;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Группа из 4 маленького или 1 большого ярлыка
    /// </summary>
    public class AppLinksGroupViewModel : ViewModel, IDropTarget
    {

        #region Id : int - Идентификатор

        /// <summary>Идентификатор</summary>
        private int _Id;

        /// <summary>Идентификатор</summary>
        public int Id
        {
            get => _Id;
            set => Set(ref _Id, value);
        }

        #endregion

        #region GroupId : int - Id группы

        /// <summary>Id группы</summary>
        private int _GroupId;

        /// <summary>Id группы</summary>
        public int GroupId
        {
            get => _GroupId;
            set => Set(ref _GroupId, value);
        }

        #endregion

        

        #region BigLinkViewModel : AppLinkViewModel - Большой ярлык

        /// <summary>Большой ярлык</summary>
        private AppLinkViewModel _BigLinkViewModel;

        /// <summary>Большой ярлык</summary>
        public AppLinkViewModel BigLinkViewModel
        {
            get => _BigLinkViewModel;
            set => Set(ref _BigLinkViewModel, value);
        }

        #endregion

        #region AppLinkViewModel1 : AppLinkViewModel - Link1

        /// <summary>Link1</summary>
        private AppLinkViewModel _AppLinkViewModel1;

        /// <summary>Link1</summary>
        public AppLinkViewModel AppLinkViewModel1
        {
            get => _AppLinkViewModel1;
            set => Set(ref _AppLinkViewModel1, value);
        }

        #endregion

        #region AppLinkViewModel2 : AppLinkViewModel - Link2

        /// <summary>Link2</summary>
        private AppLinkViewModel _AppLinkViewModel2;

        /// <summary>Link2</summary>
        public AppLinkViewModel AppLinkViewModel2
        {
            get => _AppLinkViewModel2;
            set => Set(ref _AppLinkViewModel2, value);
        }

        #endregion

        #region AppLinkViewModel3 : AppLinkViewModel - Link3

        /// <summary>Link3</summary>
        private AppLinkViewModel _AppLinkViewModel3;

        /// <summary>Link3</summary>
        public AppLinkViewModel AppLinkViewModel3
        {
            get => _AppLinkViewModel3;
            set => Set(ref _AppLinkViewModel3, value);
        }

        #endregion

        #region AppLinkViewModel4 : AppLinkViewModel - Link4

        /// <summary>Link4</summary>
        private AppLinkViewModel _AppLinkViewModel4;

        /// <summary>Link4</summary>
        public AppLinkViewModel AppLinkViewModel4
        {
            get => _AppLinkViewModel4;
            set => Set(ref _AppLinkViewModel4, value);
        }

        #endregion


        public void DragOver(IDropInfo dropInfo)
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


        public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data;

            if (sourceItem is not DataObject dataObject ||
                dataObject.GetData(DataFormats.FileDrop) is not string[] strArray) return;

            var ls = App.LinkService;

            var firstStr = strArray[0];

            var linkNumber = ((Border)dropInfo.VisualTarget).Tag as string;

            var link = ls.CreateLink(firstStr);

            switch (linkNumber)
            {
                case "1":
                    AppLinkViewModel1 = link.ToViewModel();
                    break;
                case "2":
                    AppLinkViewModel2 = link.ToViewModel();
                    break;
                case "3":
                    AppLinkViewModel3 = link.ToViewModel();
                    break;
                case "4":
                    AppLinkViewModel4 = link.ToViewModel();
                    break;
            }

            App.DataManager.UpdateAppLinkGroup(this.ToModel());

            if (strArray.Length < 2) return;

            var vm = App.MainWindowViewModel.Groups.First(g => g.Id == GroupId);

            vm.AddLinks(strArray.Skip(1).ToArray());

        }
    }
}
