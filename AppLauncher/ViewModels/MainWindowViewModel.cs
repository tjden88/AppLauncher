using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Views;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    public class MainWindowViewModel : ViewModel, IDropTarget
    {

        #region Groups : ObservableCollection<GroupViewModel> - Группы с ярлыками приложений

        /// <summary>Группы с ярлыками приложений</summary>
        private ObservableCollection<GroupViewModel> _Groups;

        /// <summary>Группы с ярлыками приложений</summary>
        public ObservableCollection<GroupViewModel> Groups
        {
            get => _Groups;
            set => Set(ref _Groups, value);
        }

        #endregion


        #region SelectedGroup : GroupViewModel - Выбранная группа

        /// <summary>Выбранная группа</summary>
        private GroupViewModel _SelectedGroup;

        /// <summary>Выбранная группа</summary>
        public GroupViewModel SelectedGroup
        {
            get => _SelectedGroup;
            set => IfSet(ref _SelectedGroup, value)
                .ThenIf(v => v != null, v => v.IsSelected = true)
                .ThenIfOld(old => old != null, old => old.IsSelected = false);
        }

        #endregion


        #region Title : string - Текст в заголовке

        /// <summary>Текст в заголовке</summary>
        private string _Title;

        /// <summary>Текст в заголовке</summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        #endregion


        #region IsTopMost : bool - Поверх всех окон

        /// <summary>Поверх всех окон</summary>
        public bool IsTopMost
        {
            get => App.SettingsWindowViewModel.IsTopMost;
            set
            {
                if(Equals(value, App.SettingsWindowViewModel.IsTopMost)) return;
                App.SettingsWindowViewModel.IsTopMost = value;
                OnPropertyChanged(nameof(IsTopMost));
            }
        }

        #endregion


        #region IsHidden : bool - Окно свёрнуто

        /// <summary>Окно свёрнуто</summary>
        private bool _IsHidden;

        /// <summary>Окно свёрнуто</summary>
        public bool IsHidden
        {
            get => _IsHidden;
            set => Set(ref _IsHidden, value);
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
            var groups = App.DataManager
                .LoadGroupsData()
                .ToArray();

            var vm = groups
                .Select(g => g.ToViewModel());

            Groups = new(vm);
            foreach (var group in Groups)
            {
                var viewModels = groups
                    .First(g => g.Id == group.Id).Cells
                    .Select(c => c.ToViewModel());

                group.ShortcutCells = new(viewModels);
            }
            App.DataManager.CanSaveData = true;

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
            var newGroup = new GroupViewModel()
            {
                Name = "Новая группа",
                Id = App.DataManager.GetNextGroupId(),
            };
            Groups.Add(newGroup);
            App.DataManager.SaveData();
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


        #region Command ChangeTopMostCommand - Изменить позицию окна

        /// <summary>Изменить позицию окна</summary>
        private Command _ChangeTopMostCommand;

        /// <summary>Изменить позицию окна</summary>
        public Command ChangeTopMostCommand => _ChangeTopMostCommand
            ??= new Command(OnChangeTopMostCommandExecuted, CanChangeTopMostCommandExecute, "Изменить позицию окна");

        /// <summary>Проверка возможности выполнения - Изменить позицию окна</summary>
        private bool CanChangeTopMostCommandExecute() => true;

        /// <summary>Логика выполнения - Изменить позицию окна</summary>
        private void OnChangeTopMostCommandExecuted() => IsTopMost = !IsTopMost;

        #endregion


        #region Command ShowSettingsWindowCommand - Показать окно настроек

        /// <summary>Показать окно настроек</summary>
        private Command _ShowSettingsWindowCommand;

        /// <summary>Показать окно настроек</summary>
        public Command ShowSettingsWindowCommand => _ShowSettingsWindowCommand
            ??= new Command(OnShowSettingsWindowCommandExecuted, CanShowSettingsWindowCommandExecute, "Показать окно настроек");

        /// <summary>Проверка возможности выполнения - Показать окно настроек</summary>
        private bool CanShowSettingsWindowCommandExecute() => true;

        /// <summary>Логика выполнения - Показать окно настроек</summary>
        private void OnShowSettingsWindowCommandExecuted()
        {
            var wnd = new SettingsWindow
            {
                Owner = App.ActiveWindow,
                DataContext = App.SettingsWindowViewModel
            };
            wnd.ShowDialog();
        }

        #endregion


        #endregion


        public void DragOver(IDropInfo dropInfo) => DragDropHelper.DragOver(dropInfo, this, DragDropHelper.DropType.All);

        public void Drop(IDropInfo dropInfo)
        {
            var dataManager = App.DataManager;

            var dropped = DragDropHelper.PerformDrop(dropInfo);

            if (dropped.ShortcutCell is { } cell)
            {
                var newGroup = new GroupViewModel
                {
                    Name = "Новая группа",
                    Id = dataManager.GetNextGroupId(),
                };

                Groups.Add(newGroup);
                cell.GroupId = newGroup.Id;
                newGroup.ShortcutCells.Add(cell);
                App.DataManager.SaveData();
            }


            if (dropped.Group is { } group)
            {
                Groups.Add(group);
                App.DataManager.SaveData();
            }

            if (dropped.Shortcuts is { Length: > 0 } shortcuts)
            {
                var newGroup = new GroupViewModel
                {
                    Name = shortcuts[0].Name,
                    Id = dataManager.GetNextGroupId(),
                };
                dataManager.CanSaveData = false;
                newGroup.AddShortcuts(shortcuts);
                Groups.Add(newGroup);
                dataManager.CanSaveData = true;
                dataManager.SaveData();
            }
        }
    }
}
