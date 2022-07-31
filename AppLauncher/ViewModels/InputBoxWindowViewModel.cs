using System.Windows;
using WPR.MVVM.Commands;
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

        #region Command AcceptCommand - Принять изменения

        /// <summary>Принять изменения</summary>
        private Command _AcceptCommand;

        /// <summary>Принять изменения</summary>
        public Command AcceptCommand => _AcceptCommand
            ??= new Command(OnAcceptCommandExecuted, CanAcceptCommandExecute, "Принять изменения");

        /// <summary>Проверка возможности выполнения - Принять изменения</summary>
        private bool CanAcceptCommandExecute(object p) => p is Window;

        /// <summary>Логика выполнения - Принять изменения</summary>
        private void OnAcceptCommandExecuted(object p)
        {
            ((Window) p).DialogResult = true;
        }

        #endregion
    }
}
