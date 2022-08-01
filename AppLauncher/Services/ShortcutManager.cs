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


        private string _ShortcutFullPath(string ShortcutPath) => Path.Combine(_ShortcutsPath, ShortcutPath);

        public ShortcutManager(IIconBuilder IconBuilder, IShortcutBuilder ShortcutBuilder)
        {
            _IconBuilder = IconBuilder;
            _ShortcutBuilder = ShortcutBuilder;
            Directory.CreateDirectory(_ShortcutsPath);
        }


        /// <summary>
        /// Получить путь до целевого объекта ярлыка
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        /// <returns></returns>
        public string GetFilePath(string ShortcutPath)
        {
            var path = _ShortcutFullPath(ShortcutPath);

            return _ShortcutBuilder.GetExecutingPath(path);
        }


        /// <summary>
        /// Создать ярлык во внутренней папке
        /// </summary>
        /// <param name="FileName">Имя к оригинальному файлу/папке</param>
        /// <returns>Созданный ярлык, не привязанный к группе</returns>
        public Shortcut CreateShortcut(string FileName)
        {

            const string linkExtension = ".lnk";

            var fileNameNoExt = Path.GetFileNameWithoutExtension(FileName);

            var newFileName = Path.Combine(_ShortcutsPath, fileNameNoExt + linkExtension);

            var intCount = 1;
            while (File.Exists(newFileName))
            {
                newFileName = Path.Combine(_ShortcutsPath, $"{fileNameNoExt}({intCount++}){linkExtension}");
            }


            var cuccess = CreateShortcut(FileName, newFileName);

            if (!cuccess) return null;


            return new Shortcut
            {
                Path = Path.GetFileName(newFileName),
                Name = fileNameNoExt,
            };

        }

        /// <summary>
        /// Удалить ярлык из рабочей папки
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        public void DeleteShortcut(string ShortcutPath) => File.Delete(_ShortcutFullPath(ShortcutPath));


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
            var path = _ShortcutFullPath(ShortcutPath);

            var iconLocation = _ShortcutBuilder.GetIconLocation(ShortcutPath) 
                               ?? _ShortcutBuilder.GetExecutingPath(ShortcutPath);

            return iconLocation == null ? null : _IconBuilder.GetImage(iconLocation);
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

        #region Private


        /// <summary>
        /// Создать ярлык для файла и сохранить на диске
        /// </summary>
        /// <param name="originalFileName">Файл, для которого необходим ярлык</param>
        /// <param name="saveFileName">Путь для сохранения (вместе с расширением .lnk)</param>
        /// <returns></returns>
        private bool CreateShortcut(string originalFileName, string saveFileName)
        {
            if (!File.Exists(originalFileName) && !Directory.Exists(originalFileName))
                throw new FileNotFoundException(originalFileName);

            try
            {
                if (Path.GetExtension(originalFileName) == ".lnk")
                    File.Copy(originalFileName, saveFileName);
                else
                {
                    using var sc = new WindowsShortcut();
                    sc.Path = originalFileName;
                    sc.WorkingDirectory = Path.GetDirectoryName(originalFileName);
                    sc.Save(saveFileName);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

        }
        
        #endregion

    }
}
