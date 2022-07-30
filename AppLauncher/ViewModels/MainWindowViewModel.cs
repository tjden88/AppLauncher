using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    internal class MainWindowViewModel : WindowViewModel
    {

        public MainWindowViewModel()
        {
            if (IsDesignMode)
            {
                AppGroupViewModels = new()
                {
                    new AppGroupViewModel {ColumnNumber = 1},
                    new AppGroupViewModel {ColumnNumber = 1},
                    new AppGroupViewModel {ColumnNumber = 2},
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


        #region AppGroupViewModels : ObservableCollection<AppGroupViewModel> - Группы с ярлыками приложений

        /// <summary>Группы с ярлыками приложений</summary>
        private ObservableCollection<AppGroupViewModel> _AppGroupViewModels;

        /// <summary>Группы с ярлыками приложений</summary>
        public ObservableCollection<AppGroupViewModel> AppGroupViewModels
        {
            get => _AppGroupViewModels;
            set => Set(ref _AppGroupViewModels, value);
        }

        #endregion


        public IEnumerable<AppGroupViewModel> Groups1Column => AppGroupViewModels.Where(g => g.ColumnNumber == 1);
        public IEnumerable<AppGroupViewModel> Groups2Column => AppGroupViewModels.Where(g => g.ColumnNumber == 2);
        public IEnumerable<AppGroupViewModel> Groups3Column => AppGroupViewModels.Where(g => g.ColumnNumber == 3);
        public IEnumerable<AppGroupViewModel> Groups4Column => AppGroupViewModels.Where(g => g.ColumnNumber == 4);
        public IEnumerable<AppGroupViewModel> Groups5Column => AppGroupViewModels.Where(g => g.ColumnNumber == 5);



    }
}
