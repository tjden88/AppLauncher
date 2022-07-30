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

        
    }
}
