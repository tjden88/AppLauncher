using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {

        public MainWindowViewModel()
        {
            //if (IsDesignMode)
            //{
            //    AppGroupViewModels = new()
            //    {
            //        new AppGroupViewModel {ColumnNumber = 1},
            //        new AppGroupViewModel {ColumnNumber = 1},
            //        new AppGroupViewModel {ColumnNumber = 2},
            //    };
            //}

            AppGroupViewModels = new()
                {
                    new AppGroupViewModel {ColumnNumber = 1},
                    new AppGroupViewModel {ColumnNumber = 1},
                    new AppGroupViewModel {ColumnNumber = 2},
                };
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


        #region SelectedGroup : AppGroupViewModel - Выбранная группа

        /// <summary>Выбранная группа</summary>
        private AppGroupViewModel _SelectedGroup;

        /// <summary>Выбранная группа</summary>
        public AppGroupViewModel SelectedGroup
        {
            get => _SelectedGroup;
            set => IfSet(ref _SelectedGroup, value)
                .ThenIf(v=> v!= null, v => v.IsSelected = true)
                .ThenIfOld(old => old !=null, old => old.IsSelected = false);
        }

        #endregion


        public IEnumerable<AppGroupViewModel> Groups1Column => AppGroupViewModels.Where(g => g.ColumnNumber == 1);
        public IEnumerable<AppGroupViewModel> Groups2Column => AppGroupViewModels.Where(g => g.ColumnNumber == 2);
        public IEnumerable<AppGroupViewModel> Groups3Column => AppGroupViewModels.Where(g => g.ColumnNumber == 3);
        public IEnumerable<AppGroupViewModel> Groups4Column => AppGroupViewModels.Where(g => g.ColumnNumber == 4);
        public IEnumerable<AppGroupViewModel> Groups5Column => AppGroupViewModels.Where(g => g.ColumnNumber == 5);



        #region Commands


        #region Command AddGroupCommand - Добавить группу

        /// <summary>Добавить группу</summary>
        private Command _AddGroupCommand;

        /// <summary>Добавить группу</summary>
        public Command AddGroupCommand => _AddGroupCommand
            ??= new Command(OnAddGroupCommandExecuted, CanAddGroupCommandExecute, "Добавить группу");

        /// <summary>Проверка возможности выполнения - Добавить группу</summary>
        private bool CanAddGroupCommandExecute() => true;

        /// <summary>Логика выполнения - Добавить группу</summary>
        private void OnAddGroupCommandExecuted()
        {
            var column = AppGroupViewModels
                .Select(vm => vm.ColumnNumber)
                .DefaultIfEmpty()
                .Max();

            var newGroup = App.DataManager.AddGroup("Новая группа", column);
            //AppGroupViewModels.Add(newGroup);
        }

        #endregion

        #region Command CloseWindowCommand - Закрыть или свернуть окно

        /// <summary>Закрыть или свернуть окно</summary>
        private Command _CloseWindowCommand;

        /// <summary>Закрыть или свернуть окно</summary>
        public Command CloseWindowCommand => _CloseWindowCommand
            ??= new Command(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecute, "Закрыть или свернуть окно");

        /// <summary>Проверка возможности выполнения - Закрыть или свернуть окно</summary>
        private bool CanCloseWindowCommandExecute() => true;

        /// <summary>Логика выполнения - Закрыть или свернуть окно</summary>
        private void OnCloseWindowCommandExecuted()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion

    }
}
