using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using AppLauncher.Models;
using AppLauncher.Services.Interfaces;

namespace AppLauncher.Services
{
    /// <summary>
    /// Сервис работы с ярлыками
    /// </summary>
    public class ShortcutManager
    {
        private readonly IIconBuilder _IconBuilder;
        private readonly IShortcutBuilder _ShortcutBuilder;
        private readonly string _ShortcutsPath = Path.Combine(Environment.CurrentDirectory, "Shortcuts");




        public ShortcutManager(IIconBuilder IconBuilder, IShortcutBuilder ShortcutBuilder)
        {
            _IconBuilder = IconBuilder;
            _ShortcutBuilder = ShortcutBuilder;
            Directory.CreateDirectory(_ShortcutsPath);
        }

        private string _ShortcutFullPath(string ShortcutPath) =>
            Path.Combine(_ShortcutsPath, ShortcutPath);


        /// <summary>
        /// Получить путь до целевого объекта ярлыка
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        /// <returns></returns>
        public string GetFilePath(string ShortcutPath) =>
            _ShortcutBuilder.GetExecutingPath(_ShortcutFullPath(ShortcutPath));


        /// <summary>
        /// Создать ярлык во внутренней папке
        /// </summary>
        /// <param name="FileName">Имя к оригинальному файлу/папке</param>
        /// <returns>Созданный ярлык, или null</returns>
        public Shortcut CreateShortcut(string FileName)
        {
            if (!File.Exists(FileName) && !Directory.Exists(FileName))
                return null;

            var shortcutName = Path.GetFileNameWithoutExtension(FileName);

            if (string.IsNullOrEmpty(shortcutName)) // Корневые каталоги
            {
                var drive = new DriveInfo(FileName[0].ToString());
                shortcutName = $"{drive.VolumeLabel} ({drive.Name})";
            }

            var fileExtension = Path.GetExtension(FileName).ToLower();

            var shortcutFileName = GetAvaliableFileName(shortcutName, fileExtension == ".url" ? ".url" : ".lnk");

            // Копируем готовые ярлыки
            if (fileExtension is ".lnk" or ".url")
            {
                try
                {
                    File.Copy(FileName, _ShortcutFullPath(shortcutFileName));

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return null;
                }
            }
            else // Создать ярлык из файла/папки
            {
                if (!_ShortcutBuilder.CreateShortcut(FileName, _ShortcutFullPath(shortcutFileName)))
                    return null;
            }


            return new Shortcut
            {
                Name = shortcutName,
                Path = shortcutFileName,
            };
        }


        // Получить свободное имя в рабочем каталоге
        // initName - имя без расширения
        private string GetAvaliableFileName(string initName, string extension)
        {

            initName = Path.GetInvalidFileNameChars()
                .Aggregate(initName, (current, c) => current
                    .Replace(c.ToString(), string.Empty));

            var newFileName = Path.Combine(_ShortcutsPath, initName + extension);


            var intCount = 1;
            while (File.Exists(newFileName))
            {
                newFileName = Path.Combine(_ShortcutsPath, $"{initName}({intCount++}){extension}");
            }

            return newFileName;
        }


        /// <summary>
        /// Удалить ярлык из рабочей папки
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        public void DeleteShortcut(string ShortcutPath) =>
            File.Delete(_ShortcutFullPath(ShortcutPath));



        /// <summary>
        /// Запустить ярлык
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        public void StartProcess(string ShortcutPath)
        {
            var path = _ShortcutFullPath(ShortcutPath);

            if (!File.Exists(path)) return;

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }



        /// <summary>
        /// Получить значок из ярлыка
        /// </summary>
        /// <param name="ShortcutPath">Путь к ярлыку</param>
        /// <returns>null, если файл или папка не найдена</returns>
        public ImageSource GetIconFromShortcut(string ShortcutPath)
        {
            var path = GetFilePath(ShortcutPath) ?? _ShortcutFullPath(ShortcutPath);


            return _IconBuilder.GetImage(path);
        }



        /// <summary> Очистка лишних ярлыков </summary>
        public void CleanNotUsedShortcuts()
        {
            var usedShortcuts = App.DataManager.LoadGroupsData()
                .SelectMany(g => g.Cells)
                .SelectMany(c => c.GetAllShortcuts())
                .Select(sc => Path.Combine(_ShortcutsPath, sc.Path));

            var allShortcuts = Directory.EnumerateFiles(_ShortcutsPath);

            var notUsedFiles = allShortcuts.Except(usedShortcuts);

            foreach (var notUsedFile in notUsedFiles)
                File.Delete(notUsedFile);
        }
    }
}
