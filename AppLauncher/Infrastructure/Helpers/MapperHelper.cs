using AppLauncher.Models;
using AppLauncher.ViewModels;

namespace AppLauncher.Infrastructure.Helpers
{
    /// <summary>
    /// Методы - расширения для маппинга моделей и вьюмоделей
    /// </summary>
    public static class MapperHelper
    {
        public static GroupViewModel ToViewModel(this Group Model)
        {
            return new GroupViewModel
            {
                Id = Model.Id,
                Name = Model.Name,
            };
        }

        public static AppLinksGroupViewModel ToViewModel(this ShortcutCell LinkGroup) =>
            new ()
            {
                AppLinkViewModel1 = ToViewModel(LinkGroup.Link1),
                AppLinkViewModel2 = ToViewModel(LinkGroup.Link2),
                AppLinkViewModel3 = ToViewModel(LinkGroup.Link3),
                AppLinkViewModel4 = ToViewModel(LinkGroup.Link4),
                BigLinkViewModel = ToViewModel(LinkGroup.BigLink),
                Id = LinkGroup.Id,
                GroupId = LinkGroup.GroupId,
            };

        public static AppLinkViewModel ToViewModel(this Shortcut Link)
        {
            if (Link == null) return null;

            return new AppLinkViewModel
            {
                FilePath = Link.Path,
                Name = Link.Name,
            };
        }

        public static Shortcut ToModel(this AppLinkViewModel Link)
        {
            if (Link == null) return null;

            return new Shortcut
            {
                Path = Link.FilePath,
                Name = Link.Name,
            };
        }

        public static ShortcutCell ToModel(this AppLinksGroupViewModel vm)
        {
            return new ShortcutCell
            {
                Link1 = vm.AppLinkViewModel1.ToModel(),
                Link2 = vm.AppLinkViewModel2.ToModel(),
                Link3 = vm.AppLinkViewModel3.ToModel(),
                Link4 = vm.AppLinkViewModel4.ToModel(),
                BigLink = vm.BigLinkViewModel.ToModel(),
                Id = vm.Id,
                GroupId = vm.GroupId,
            };
        }
    }
}
