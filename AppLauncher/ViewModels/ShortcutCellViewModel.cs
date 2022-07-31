using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Models;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Группа из 4 маленького или 1 большого ярлыка
    /// </summary>
    public class ShortcutCellViewModel : ViewModel, IDropTarget
    {
        private bool _IsEmpty() => BigLinkViewModel == null && ShortcutViewModel1 == null && ShortcutViewModel2 == null && ShortcutViewModel3 == null && ShortcutViewModel4 == null;

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

        
        #region BigLinkViewModel : ShortcutViewModel - Большой ярлык

        /// <summary>Большой ярлык</summary>
        private ShortcutViewModel _BigLinkViewModel;

        /// <summary>Большой ярлык</summary>
        public ShortcutViewModel BigLinkViewModel
        {
            get => _BigLinkViewModel;
            set => Set(ref _BigLinkViewModel, value);
        }

        #endregion

        #region ShortcutViewModel1 : ShortcutViewModel - Link1

        /// <summary>Link1</summary>
        private ShortcutViewModel _ShortcutViewModel1;

        /// <summary>Link1</summary>
        public ShortcutViewModel ShortcutViewModel1
        {
            get => _ShortcutViewModel1;
            set => Set(ref _ShortcutViewModel1, value);
        }

        #endregion

        #region ShortcutViewModel2 : ShortcutViewModel - Link2

        /// <summary>Link2</summary>
        private ShortcutViewModel _ShortcutViewModel2;

        /// <summary>Link2</summary>
        public ShortcutViewModel ShortcutViewModel2
        {
            get => _ShortcutViewModel2;
            set => Set(ref _ShortcutViewModel2, value);
        }

        #endregion

        #region ShortcutViewModel3 : ShortcutViewModel - Link3

        /// <summary>Link3</summary>
        private ShortcutViewModel _ShortcutViewModel3;

        /// <summary>Link3</summary>
        public ShortcutViewModel ShortcutViewModel3
        {
            get => _ShortcutViewModel3;
            set => Set(ref _ShortcutViewModel3, value);
        }

        #endregion

        #region ShortcutViewModel4 : ShortcutViewModel - Link4

        /// <summary>Link4</summary>
        private ShortcutViewModel _ShortcutViewModel4;

        /// <summary>Link4</summary>
        public ShortcutViewModel ShortcutViewModel4
        {
            get => _ShortcutViewModel4;
            set => Set(ref _ShortcutViewModel4, value);
        }

        #endregion


        public List<ShortcutViewModel> GetAllShortcuts()
        {
            var result = new List<ShortcutViewModel>();
            if (ShortcutViewModel1 != null)
                result.Add(ShortcutViewModel1);
            if (ShortcutViewModel2 != null)
                result.Add(ShortcutViewModel2);
            if (ShortcutViewModel3 != null)
                result.Add(ShortcutViewModel3);
            if (ShortcutViewModel4 != null)
                result.Add(ShortcutViewModel4);
            if (BigLinkViewModel != null)
                result.Add(BigLinkViewModel);

            return result;
        }

        public void Remove(ShortcutViewModel vm)
        {
            if (ReferenceEquals(ShortcutViewModel1, vm))
                ShortcutViewModel1 = null;
            if (ReferenceEquals(ShortcutViewModel2, vm))
                ShortcutViewModel2 = null;
            if (ReferenceEquals(ShortcutViewModel3, vm))
                ShortcutViewModel3 = null;
            if (ReferenceEquals(ShortcutViewModel4, vm))
                ShortcutViewModel4 = null;
            if (ReferenceEquals(BigLinkViewModel, vm))
                BigLinkViewModel = null;

        }
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
                vm.ShortcutCells.Remove(this);
                App.DataManager.DeleteCell(Id);
            }
        }

        #endregion

        #endregion


        #region DragDrop

        public void DragOver(IDropInfo dropInfo) => DragDropHelper.DragOver(dropInfo);


        public void Drop(IDropInfo dropInfo)
        {
            var links = DragDropHelper.Drop(dropInfo);


            var firstLink = links[0];

            var linkNumber = ((Border)dropInfo.VisualTarget).Tag as string;

            switch (linkNumber)
            {
                case "1":
                    ShortcutViewModel1 = firstLink.ToViewModel();
                    break;
                case "2":
                    ShortcutViewModel2 = firstLink.ToViewModel();
                    break;
                case "3":
                    ShortcutViewModel3 = firstLink.ToViewModel();
                    break;
                case "4":
                    ShortcutViewModel4 = firstLink.ToViewModel();
                    break;
            }

            App.DataManager.UpdateCell(this.ToModel());

            if (links.Length < 2) return;

            var vm = App.MainWindowViewModel.Groups.First(g => g.Id == GroupId);

            vm.AddLinks(links.Skip(1).ToArray());

        } 

        #endregion

    }
}
