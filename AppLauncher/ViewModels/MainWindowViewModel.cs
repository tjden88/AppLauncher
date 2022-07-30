using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AppLauncher.Models;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    public class MainWindowViewModel : WindowViewModel
    {


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


        #region Groups : ObservableCollection<AppGroupViewModel> - Группы с ярлыками приложений

        /// <summary>Группы с ярлыками приложений</summary>
        private ObservableCollection<AppGroupViewModel> _Groups;

        /// <summary>Группы с ярлыками приложений</summary>
        public ObservableCollection<AppGroupViewModel> Groups
        {
            get => _Groups;
            set => Set(ref _Groups, value);
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
                .ThenIf(v => v != null, v => v.IsSelected = true)
                .ThenIfOld(old => old != null, old => old.IsSelected = false);
        }

        #endregion


        #region Commands

        #region Command LoadGroupsCommand - Загрузить группы

        /// <summary>Загрузить группы</summary>
        private Command _LoadGroupsCommand;

        /// <summary>Загрузить группы</summary>
        public Command LoadGroupsCommand => _LoadGroupsCommand
            ??= new Command(OnLoadGroupsCommandExecuted, CanLoadGroupsCommandExecute, "Загрузить группы");

        /// <summary>Проверка возможности выполнения - Загрузить группы</summary>
        private bool CanLoadGroupsCommandExecute() => true;

        /// <summary>Логика выполнения - Загрузить группы</summary>
        private void OnLoadGroupsCommandExecuted()
        {
            var groups = App.DataManager.LoadGroups();
            var vm = groups.Select(MapModel);
            Groups = new(vm);
        }

        #endregion

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
            var newGroup = App.DataManager.AddGroup("Новая группа");
            Groups.Add(MapModel(newGroup));
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


        private AppGroupViewModel MapModel(Group Model)
        {
            return new AppGroupViewModel
            {
                Id = Model.Id,
                Name = Model.Name,
            };
        }
    }
}
