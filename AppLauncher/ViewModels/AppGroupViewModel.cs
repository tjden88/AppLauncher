using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AppLauncher.Services;
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
    private ObservableCollection<AppLinkViewModel> _Links = new();

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

        if (sourceItem is DataObject dataObject && dataObject.GetData(DataFormats.FileDrop) is string[])
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Copy;
            return;
        }

        dropInfo.Effects = DragDropEffects.None;
    }

    public void Drop(IDropInfo dropInfo)
    {
        var sourceItem = dropInfo.Data;

        if (sourceItem is not DataObject dataObject ||
            dataObject.GetData(DataFormats.FileDrop) is not string[] strArray) return;


        var links = strArray.Select(str => App.LinkService.CreateLink(str));

        var addedLinks = links.Select(l => new AppLinkViewModel
        {
            Name = l.Name,
            FilePath = l.Path,
            Image = new ShortcutService().GetIconFromShortcut(l.Path),
        });

        foreach (var draggedLink in addedLinks)
        {
            Links.Add(draggedLink);
        }

    }
}
