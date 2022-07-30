using System.Collections.ObjectModel;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels;

public class AppGroupViewModel : ViewModel, IDropTarget
{

    #region Name : string - Имя группы

    /// <summary>Имя группы</summary>
    private string _Name = "Новая группа";

    /// <summary>Имя группы</summary>
    public string Name
    {
        get => _Name;
        set => Set(ref _Name, value);
    }

    #endregion


    #region IsSelected : bool - Выбрана ли группа для редактирования

    /// <summary>Выбрана ли группа для редактирования</summary>
    private bool _IsSelected;

    /// <summary>Выбрана ли группа для редактирования</summary>
    public bool IsSelected
    {
        get => _IsSelected;
        set => Set(ref _IsSelected, value);
    }

    #endregion


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

    #endregion

    public void DragOver(IDropInfo dropInfo)
    {
        var sourceItem = dropInfo.Data;
        var targetItem = dropInfo.TargetItem;

        if (sourceItem != null && targetItem != null)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Copy;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        var sourceItem = dropInfo.Data;
        var targetItem = dropInfo.TargetItem;
    }
}
