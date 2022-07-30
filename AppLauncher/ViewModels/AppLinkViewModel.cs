using System;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;
using Shell32;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Модель - представление ярлыка для запуска
    /// </summary>
    public class AppLinkViewModel : ViewModel
    {

        #region Name : string - Имя ярлыка

        /// <summary>Имя ярлыка</summary>
        private string _Name;

        /// <summary>Имя ярлыка</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion

        #region FilePath : string - Путь для запуска

        /// <summary>Путь для запуска</summary>
        private string _FilePath;

        /// <summary>Путь для запуска</summary>
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        #endregion

        #region Command LaunchCommand - Запуск

        /// <summary>Запуск</summary>
        private Command _LaunchCommand;

        /// <summary>Запуск</summary>
        public Command LaunchCommand => _LaunchCommand
            ??= new Command(OnLaunchCommandExecuted, CanLaunchCommandExecute, "Запуск");

        /// <summary>Проверка возможности выполнения - Запуск</summary>
        private bool CanLaunchCommandExecute() => true;

        /// <summary>Логика выполнения - Запуск</summary>
        private void OnLaunchCommandExecuted()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = FilePath,
                UseShellExecute = true
            });
        }

        #endregion

        /// <summary> Создать вьюмодель из ссылки на файл / ярлык / папку </summary>
        public static AppLinkViewModel CreateLinkViewModelFromLink(string Url)
        {
            var name = Path.GetFileName(Url);
            var extension = Path.GetExtension(Url);

            var path = extension switch
            {
                ".lnk" => GetShortcutTargetFile2(Url),
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

            var link = (Shell32.ShellLinkObject)folderItem.GetLink;
            return link.Path;
        }

        private static string GetShortcutTargetFile2(string shortcutFilename)
        {
            WshShell shell = new WshShell();
            WshShortcut shortcut = (WshShortcut)shell.CreateShortcut(@"C:\SomeShortcut.lnk");
            return shortcut.TargetPath;
        }


    }
}
