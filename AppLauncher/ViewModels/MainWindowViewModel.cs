using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AppLauncher.Views;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    internal class MainWindowViewModel : WindowViewModel
    {

        public MainWindowViewModel()
        {
            if (IsDesignMode)
            {
                AppGroups = new()
                {
                    new AppGroup() {ColumnNumber = 1},
                    new AppGroup() {ColumnNumber = 1},
                    new AppGroup() {ColumnNumber = 2},
                };
            }
        }


        #region ColumnsCount : int - Количество колонок

        /// <summary>Количество колонок</summary>
        private int _ColumnsCount = 3;

        /// <summary>Количество колонок</summary>
        public int ColumnsCount
        {
            get => _ColumnsCount;
            set => Set(ref _ColumnsCount, value);
        }

        #endregion


        #region AppGroups : ObservableCollection<AppGroup> - Группы с ярлыками приложений

        /// <summary>Группы с ярлыками приложений</summary>
        private ObservableCollection<AppGroup> _AppGroups;

        /// <summary>Группы с ярлыками приложений</summary>
        public ObservableCollection<AppGroup> AppGroups
        {
            get => _AppGroups;
            set => Set(ref _AppGroups, value);
        }

        #endregion


        public IEnumerable<AppGroup> Groups1Column => AppGroups.Where(g => g.ColumnNumber == 1);
        public IEnumerable<AppGroup> Groups2Column => AppGroups.Where(g => g.ColumnNumber == 2);
        public IEnumerable<AppGroup> Groups3Column => AppGroups.Where(g => g.ColumnNumber == 3);
        public IEnumerable<AppGroup> Groups4Column => AppGroups.Where(g => g.ColumnNumber == 4);
        public IEnumerable<AppGroup> Groups5Column => AppGroups.Where(g => g.ColumnNumber == 5);



    }
}
