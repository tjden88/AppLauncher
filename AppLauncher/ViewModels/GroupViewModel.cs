using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Views;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels;

/// <summary>
/// Группа ярлыков в главнгом окне
/// </summary>
public class GroupViewModel : ViewModel, IDropTarget
{
    /// <summary>
    /// Пустая вьюмодель с нулевым идентификатором для добавления новой ячейки в группу
    /// </summary>
    public static ShortcutCellViewModel MockShortcutCellViewModel { get; } = new();


    #region Id : int - Id группы

    /// <summary>Id группы</summary>
    private int _Id;

    /// <summary>Id группы</summary>
    public int Id
    {
        get => _Id;
        set => Set(ref _Id, value);
    }

    #endregion


    #region Name : string - Имя группы

    /// <summary>Имя группы</summary>
    private string _Name = "Новая группа";

    /// <summary>Имя группы</summary>
    public string Name
    {
        get => _Name;
        set => IfSet(ref _Name, value);
    }

    #endregion


    #region IsSelected : bool - Выбрана ли группа для редактирования

    /// <summary>Выбрана ли группа для редактирования</summary>
    private bool _IsSelected;

    /// <summary>Выбрана ли группа для редактирования</summary>
    public bool IsSelected
    {
        get => _IsSelected;
        set => IfSet(ref _IsSelected, value).Then(v =>
        {
            if(v)
                ShortcutCells.Add(MockShortcutCellViewModel);
            else
                ShortcutCells.Remove(MockShortcutCellViewModel);
        });
    }

    #endregion


    #region ShortcutCells : ObservableCollection<ShortcutViewModel> - Ячейки с ярлыками

    /// <summary>Ячейки с ярлыками</summary>
    private ObservableCollection<ShortcutCellViewModel> _ShortcutCells = new();

    /// <summary>Ячейки с ярлыками</summary>
    public ObservableCollection<ShortcutCellViewModel> ShortcutCells
    {
        get => _ShortcutCells;
        set => Set(ref _ShortcutCells, value);
    }

    #endregion


    #region Commands
    

    #region Command SelectGroupCommand - Выбрать группу

    /// <summary>Выбрать группу</summary>
    private Command _SelectGroupCommand;

    /// <summary>Выбрать группу</summary>
    public Command SelectGroupCommand => _SelectGroupCommand
        ??= new Command(OnSelectGroupCommandExecuted, CanSelectGroupCommandExecute, "Выбрать группу");

    /// <summary>Проверка возможности выполнения - Выбрать группу</summary>
    private bool CanSelectGroupCommandExecute() => true;

    /// <summary>Логика выполнения - Выбрать группу</summary>
    private void OnSelectGroupCommandExecuted() => App.MainWindowViewModel.SelectedGroup = !IsSelected ? this : null;

    #endregion


    #region Command RenameCommand - Переименовать группу

    /// <summary>Переименовать группу</summary>
    private Command _RenameCommand;

    /// <summary>Переименовать группу</summary>
    public Command RenameCommand => _RenameCommand
        ??= new Command(OnRenameCommandExecuted, CanRenameCommandExecute, "Переименовать группу");

    /// <summary>Проверка возможности выполнения - Переименовать группу</summary>
    private bool CanRenameCommandExecute() => true;

    /// <summary>Логика выполнения - Переименовать группу</summary>
    private void OnRenameCommandExecuted()
    {
        var vm = new InputBoxWindowViewModel()
        {
            Caption = $"Введите новое имя для группы {Name}:",
            Result = Name
        };
        var wnd = new InputBoxWindow()
        {
            Owner = App.ActiveWindow,
            DataContext = vm,
        };
        if (wnd.ShowDialog() != true) return;

        Name = vm.Result;

        App.DataManager.SaveData();
    }

    #endregion


    #region Command DeleteGroupCommand - Удалить группу

    /// <summary>Удалить группу</summary>
    private Command _DeleteGroupCommand;

    /// <summary>Удалить группу</summary>
    public Command DeleteGroupCommand => _DeleteGroupCommand
        ??= new Command(OnDeleteGroupCommandExecuted, CanDeleteGroupCommandExecute, "Удалить группу");

    /// <summary>Проверка возможности выполнения - Удалить группу</summary>
    private bool CanDeleteGroupCommandExecute() => true;

    /// <summary>Логика выполнения - Удалить группу</summary>
    private void OnDeleteGroupCommandExecuted()
    {

        var msg = !ShortcutCells.SelectMany(s=>s.GetAllShortcuts()).Any() ||
                  MessageBox.Show(App.ActiveWindow, $"Удалить группу {Name} и все ярлыки?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

        if (!msg) return;

        foreach (var cell in ShortcutCells)
           cell.GetAllShortcuts()
               .ForEach(sc => App.ShortcutService
                   .DeleteShortcut(sc.ShortcutPath));


        App.MainWindowViewModel.Groups.Remove(this);
        App.DataManager.SaveData();
    }

    #endregion


    #endregion



    public void DragOver(IDropInfo dropInfo) => DragDropHelper.DragOver(dropInfo, !IsSelected);


    public void Drop(IDropInfo dropInfo)
    {
        if (DragDropHelper.DropGroup(dropInfo) is { } group)
        {
            var index = App.MainWindowViewModel.Groups.IndexOf(this);
            App.MainWindowViewModel.Groups.Insert(index, group);
            App.DataManager.SaveData();
            return;
        }
        AddShortcuts(DragDropHelper.Drop(dropInfo));
    }


    /// <summary> Добавить ярлыки в группу и разложить по новым ячейкам </summary>
    public void AddShortcuts(ShortcutViewModel[] shortcuts)
    {
        var currentIndex = 0;

        bool CheckEnd(ShortcutCellViewModel group)
        {
            if (currentIndex == shortcuts.Length)
            {
                ShortcutCells.Add(group);
                return true;
            }
            return false;
        }

        var dataManager = App.DataManager;

        while (currentIndex < shortcuts.Length)
        {
            var newCell = new ShortcutCellViewModel
            {
                Id = dataManager.GetNextCellId(),
                GroupId = Id,
            };

            newCell.ShortcutViewModel1 = shortcuts[currentIndex++];
            if (CheckEnd(newCell)) break;

            newCell.ShortcutViewModel2 = shortcuts[currentIndex++];
            if (CheckEnd(newCell)) break;

            newCell.ShortcutViewModel3 = shortcuts[currentIndex++];
            if (CheckEnd(newCell)) break;

            newCell.ShortcutViewModel4 = shortcuts[currentIndex++];
            if (CheckEnd(newCell)) break;

            ShortcutCells.Add(newCell);
        }
        dataManager.SaveData();
    }
}
