using System.IO;
using AppLauncher.ViewModels;
using IWshRuntimeLibrary;
using Shell32;

namespace AppLauncher.Services
{
    /// <summary>
    /// Создание, обработка ярлыков.
    /// Работа с файлами.
    /// Запуск ярлыков
    /// </summary>
    internal static class LinkService
    {
        /// <summary> Создать вьюмодель из ссылки на файл / ярлык / папку </summary>
        public static AppLinkViewModel CreateLinkViewModelFromLink(string Url)
        {
            var name = Path.GetFileName(Url);
            var extension = Path.GetExtension(Url);

            var path = extension switch
            {
                ".lnk" => GetShortcutTargetFile(Url),
                _ => Url
            };
            return new AppLinkViewModel
            {
                FilePath = path,
                Name = name,
            };


        }

        private static string GetShortcutTargetFile(string shortcutFilename)
        {
            var pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            var filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            var shell = new Shell();
            var folder = shell.NameSpace(pathOnly);
            var folderItem = folder.ParseName(filenameOnly);

            if (folderItem == null) return string.Empty;

            var link = (Shell32.ShellLinkObject) folderItem.GetLink;
            return link.Path;
        }

        private static string GetShortcutTargetFile2(string shortcutFilename)
        {
            WshShell shell = new WshShell();
            WshShortcut shortcut = (WshShortcut) shell.CreateShortcut(@"C:\SomeShortcut.lnk");
            return shortcut.TargetPath;
        }
    }
}
