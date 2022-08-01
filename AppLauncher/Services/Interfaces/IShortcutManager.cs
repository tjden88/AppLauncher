using AppLauncher.Models;

namespace AppLauncher.Services.Interfaces
{
    /// <summary>
    /// Создание и обработка ярлыков
    /// </summary>
    interface IShortcutManager
    {
        /// <summary>
        /// Создать ярлык
        /// </summary>
        /// <param name="Path">Путь к файлу</param>
        public Shortcut CreateShortcut(string Path);
    }
}
