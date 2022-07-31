﻿using AppLauncher.Models;
using AppLauncher.ViewModels;

namespace AppLauncher.Infrastructure.Helpers
{
    /// <summary>
    /// Методы - расширения для маппинга моделей и вьюмоделей
    /// </summary>
    public static class MapperHelper
    {
        public static Group ToModel(this GroupViewModel vm)
        {
            return new Group
            {
                Id = vm.Id,
                Name = vm.Name,
            };
        }


        public static GroupViewModel ToViewModel(this Group Model)
        {
            return new GroupViewModel
            {
                Id = Model.Id,
                Name = Model.Name,
            };
        }

        public static ShortcutCellViewModel ToViewModel(this ShortcutCell LinkGroup) =>
            new ()
            {
                ShortcutViewModel1 = ToViewModel(LinkGroup.Link1),
                ShortcutViewModel2 = ToViewModel(LinkGroup.Link2),
                ShortcutViewModel3 = ToViewModel(LinkGroup.Link3),
                ShortcutViewModel4 = ToViewModel(LinkGroup.Link4),
                BigShortcutViewModel = ToViewModel(LinkGroup.BigLink),
                Id = LinkGroup.Id,
                GroupId = LinkGroup.GroupId,
            };

        public static ShortcutViewModel ToViewModel(this Shortcut Link)
        {
            if (Link == null) return null;

            return new ShortcutViewModel
            {
                ShortcutPath = Link.Path,
                Name = Link.Name,
            };
        }

        public static Shortcut ToModel(this ShortcutViewModel Link)
        {
            if (Link == null) return null;

            return new Shortcut
            {
                Path = Link.ShortcutPath,
                Name = Link.Name,
            };
        }

        public static ShortcutCell ToModel(this ShortcutCellViewModel vm)
        {
            return new ShortcutCell
            {
                Link1 = vm.ShortcutViewModel1.ToModel(),
                Link2 = vm.ShortcutViewModel2.ToModel(),
                Link3 = vm.ShortcutViewModel3.ToModel(),
                Link4 = vm.ShortcutViewModel4.ToModel(),
                BigLink = vm.BigShortcutViewModel.ToModel(),
                Id = vm.Id,
                GroupId = vm.GroupId,
            };
        }
    }
}
