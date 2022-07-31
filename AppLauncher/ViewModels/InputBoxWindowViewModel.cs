using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    public class InputBoxWindowViewModel : WindowViewModel
    {

        public InputBoxWindowViewModel()
        {
            Title = "Переименовать";
        }

        #region Caption : string - Описание

        /// <summary>Описание</summary>
        private string _Caption;

        /// <summary>Описание</summary>
        public string Caption
        {
            get => _Caption;
            set => Set(ref _Caption, value);
        }

        #endregion


        #region Result : string - Результат

        /// <summary>Результат</summary>
        private string _Result;

        /// <summary>Результат</summary>
        public string Result
        {
            get => _Result;
            set => Set(ref _Result, value);
        }

        #endregion


    }
}
