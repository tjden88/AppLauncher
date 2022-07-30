using System;
using System.IO;
using AppLauncher.Models;

namespace AppLauncher.Services
{
    /// <summary>
    /// Создание, обработка ярлыков.
    /// Работа с файлами.
    /// Запуск ярлыков
    /// </summary>
    public class LinkService
    {
        private readonly ShortcutCreator _ShortcutCreator;

        private readonly string _LinkPath = Path.Combine(Environment.CurrentDirectory, "Links");

        public LinkService(ShortcutCreator ShortcutCreator)
        {
            _ShortcutCreator = ShortcutCreator;
            Directory.CreateDirectory(_LinkPath);
        }

        /// <summary>
        /// Создать ярлык во внутренней папке
        /// </summary>
        /// <param name="FileName">Имя к оригинальному файлу/папке</param>
        /// <returns>Созданный ярлык, не привязанный к группе</returns>
        public AppLink CreateLink(string FileName)
        {

            const string linkExtension = ".lnk";

            var fileNameNoExt = Path.GetFileNameWithoutExtension(FileName);

            var newFileName = Path.Combine(_LinkPath, fileNameNoExt + linkExtension);

            var intCount = 1;
            while (File.Exists(newFileName))
            {
                newFileName = Path.Combine(_LinkPath, $"{fileNameNoExt} ({intCount++}){linkExtension}");
            }


            _ShortcutCreator.CreateShortcut(FileName, newFileName);


            return new AppLink
            {
                Path = newFileName,
                Name = fileNameNoExt,
            };

        }

    }
}
