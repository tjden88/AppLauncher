using System.Collections.ObjectModel;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    public class AppGroupViewModel : ViewModel
    {

        #region ColumnNumber : int - Номер колонки этой группы

        /// <summary>Номер колонки этой группы</summary>
        private int _ColumnNumber;

        /// <summary>Номер колонки этой группы</summary>
        public int ColumnNumber
        {
            get => _ColumnNumber;
            set => Set(ref _ColumnNumber, value);
        }

        #endregion

        #region Links : ObservableCollection<AppLinkViewModel> - Список ярлыков группы

        /// <summary>Список ярлыков группы</summary>
        private ObservableCollection<AppLinkViewModel> _Links;

        /// <summary>Список ярлыков группы</summary>
        public ObservableCollection<AppLinkViewModel> Links
        {
            get => _Links;
            set => Set(ref _Links, value);
        }

        #endregion

        
    }
}
