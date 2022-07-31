using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Группа из 4 маленького или 1 большого ярлыка
    /// </summary>
    public class AppLinksGroupViewModel : ViewModel
    {

        #region AppLinkViewModel1 : AppLinkViewModel - Первый (или большой и единственный) ярлык

        /// <summary>Первый (или большой и единственный) ярлык</summary>
        private AppLinkViewModel _AppLinkViewModel1;

        /// <summary>Первый (или большой и единственный) ярлык</summary>
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

    }
}
