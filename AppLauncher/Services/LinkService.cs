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
            var link = new AppLink();

            //if (Directory.Exists(FileName)) // Папка
            //{
            //    link.IsDirectory = true;
            //    link.Path = FileName;
            //    link.Name = Path.GetDirectoryName(FileName);
            //    return link;
            //}

            if (File.Exists(FileName)) // Файл
            {

                var newFileName = Path.Combine(_LinkPath, FileName);
                if (File.Exists(newFileName))
                {
                    var modifiedFileName = Path.GetFileNameWithoutExtension(FileName) + Guid.NewGuid() + Path.GetExtension(FileName);
                    newFileName = Path.Combine(_LinkPath, modifiedFileName);
                }

                var extension = Path.GetExtension(FileName);
                if (extension == ".lnk")
                {
                    File.Copy(FileName, newFileName);
                }
                else
                {
                    _ShortcutCreator.CreateShortcut(FileName, newFileName);
                }

                link.Path = newFileName;
                link.Name = Path.GetFileNameWithoutExtension(FileName);

                return link;
            }

            throw new FileNotFoundException(FileName);
        }

    }
}
