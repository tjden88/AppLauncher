using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppLauncher.Infrastructure.Helpers;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.Commands;
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

        private bool _IsEmpty() => BigLinkViewModel == null && AppLinkViewModel1 == null && AppLinkViewModel2 == null && AppLinkViewModel3 == null && AppLinkViewModel4 == null;


        #region Commands

        #region Command DeleteCommand - Удалить группу

        /// <summary>Удалить группу</summary>
        private Command _DeleteCommand;

        /// <summary>Удалить группу</summary>
        public Command DeleteCommand => _DeleteCommand
            ??= new Command(OnDeleteCommandExecuted, CanDeleteCommandExecute, "Удалить группу");

        /// <summary>Проверка возможности выполнения - Удалить группу</summary>
        private bool CanDeleteCommandExecute() => true;

        /// <summary>Логика выполнения - Удалить группу</summary>
        private void OnDeleteCommandExecuted()
        {
            if (_IsEmpty() || MessageBox.Show(App.ActiveWindow, "Удалить группу вместе с ярлыками?", "Вопрос", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                var vm = App.MainWindowViewModel.Groups.First(g => g.Id == GroupId);
                vm.LinksGroups.Remove(this);
                App.DataManager.DeleteAppLinkGroup(Id);
            }
        }

        #endregion

        #endregion

        public void DragOver(IDropInfo dropInfo) => DragDropHelper.DragOver(dropInfo);


        public void Drop(IDropInfo dropInfo)
        {
            var links = DragDropHelper.Drop(dropInfo);


            var firstLink = links[0];

            var linkNumber = ((Border)dropInfo.VisualTarget).Tag as string;

            switch (linkNumber)
            {
                case "1":
                    AppLinkViewModel1 = firstLink.ToViewModel();
                    break;
                case "2":
                    AppLinkViewModel2 = firstLink.ToViewModel();
                    break;
                case "3":
                    AppLinkViewModel3 = firstLink.ToViewModel();
                    break;
                case "4":
                    AppLinkViewModel4 = firstLink.ToViewModel();
                    break;
            }

            App.DataManager.UpdateAppLinkGroup(this.ToModel());

            if (links.Length < 2) return;

            var vm = App.MainWindowViewModel.Groups.First(g => g.Id == GroupId);

            vm.AddLinks(links.Skip(1).ToArray());

        }
    }
}
