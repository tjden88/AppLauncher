﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Models;
using GongSolutions.Wpf.DragDrop;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels;

/// <summary>
/// Группа ярлыков в главнгом окне
/// </summary>
public class GroupViewModel : ViewModel, IDropTarget
{

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
        set => IfSet(ref _Name, value)
            .Then(App.DataManager.SaveData);
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


    #region ShortcutCells : ObservableCollection<ShortcutViewModel> - Группированные ссылки

    /// <summary>Группированные ссылки</summary>
    private ObservableCollection<ShortcutCellViewModel> _ShortcutCells = new();

    /// <summary>Группированные ссылки</summary>
    public ObservableCollection<ShortcutCellViewModel> ShortcutCells
    {
        get => _ShortcutCells;
        set => Set(ref _ShortcutCells, value);
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
        var links = App.DataManager.LoadGroupCells(Id).ToArray();
        var vm = links.Select(l => l.ToViewModel());
        ShortcutCells = new(vm);
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
        var msg = ShortcutCells?.Count == 0 ||
                  MessageBox.Show(App.ActiveWindow, $"Удалить группу {Name} и все ярлыки?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

        if (!msg) return;

        App.DataManager.DeleteGroup(Id);
        App.MainWindowViewModel.Groups.Remove(this);
    }

    #endregion

    #endregion



    public void DragOver(IDropInfo dropInfo) => DragDropHelper.DragOver(dropInfo);


    public void Drop(IDropInfo dropInfo)
    {

        AddLinks(DragDropHelper.Drop(dropInfo));
    }

    public void AddLinks(Shortcut[] links)
    {

        var currentIndex = 0;

        bool CheckEnd(ShortcutCell group)
        {
            if (currentIndex == links.Length)
            {
                ShortcutCells.Add(group.ToViewModel());
                return true;
            }
            return false;
        }


        while (currentIndex < links.Length)
        {
            var newGroup = dataManager.AddCell(Id);

            newGroup.Link1 = links[currentIndex++];
            if (CheckEnd(newGroup)) break;

            newGroup.Link2 =links[currentIndex++];
            if (CheckEnd(newGroup)) break;

            newGroup.Link3 = links[currentIndex++];
            if (CheckEnd(newGroup)) break;

            newGroup.Link4 = links[currentIndex++];
            if (CheckEnd(newGroup)) break;

            dataManager.UpdateCell(newGroup);
            ShortcutCells.Add(newGroup.ToViewModel());
        }
    }
}
