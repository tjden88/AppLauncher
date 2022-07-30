using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AppLauncher.Models;
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

    #region Command LoadLinksCommand - Загрузить ярлыки группы

    /// <summary>Загрузить ярлыки группы</summary>
    private Command _LoadLinksCommand;

    /// <summary>Загрузить ярлыки группы</summary>
    public Command LoadLinksCommand => _LoadLinksCommand
        ??= new Command(OnLoadLinksCommandExecuted, CanLoadLinksCommandExecute, "Загрузить ярлыки группы");

    /// <summary>Проверка возможности выполнения - Загрузить ярлыки группы</summary>
    private bool CanLoadLinksCommandExecute() => true;

    /// <summary>Логика выполнения - Загрузить ярлыки группы</summary>
    private void OnLoadLinksCommandExecuted()
    {
        var links = App.DataManager.LoadGroupLinks(Id).ToArray();
        var vm = links.Select(MapModel);
        Links=new(vm);
    }

    #endregion


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
        var msg = MessageBox.Show(App.ActiveWindow, 
            $"Удалить группу {Name} и все ярлыки?", "Внимание!",
            MessageBoxButton.YesNo);
        if (msg != MessageBoxResult.Yes) return;

        App.DataManager.DeleteGroup(Id);
        App.MainWindowViewModel.Groups.Remove(this);
    }

    #endregion

    #endregion


    private static AppLinkViewModel MapModel(AppLink Link)
    {
       return new AppLinkViewModel
        {
            FilePath = Link.Path,
            Name = Link.Name,
        };
    }


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

        var dataManager = App.DataManager;

        foreach (var str in strArray)
        {
            var added = dataManager.AddAppLink(str, Id);
            Links.Add(MapModel(added));
        }

    }
}
