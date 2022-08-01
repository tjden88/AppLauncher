namespace AppLauncher.Services.Interfaces
{
    /// <summary>
    /// Создание и обработка ярлыков
    /// </summary>
    public interface IShortcutBuilder
    {
        /// <summary>
        /// Создать ярлык
        /// </summary>
        /// <param name="PathFrom">Путь к файлу</param>
        /// <param name="SaveTo">Путь для сохранения</param>
        public bool CreateShortcut(string PathFrom, string SaveTo);

        /// <summary>
        /// Получить исполняемый файл ярлыка
        /// </summary>
        /// <param name="shortcutPath">Путь к ярлыку</param>
        public string GetExecutingPath(string shortcutPath);

        /// <summary>
        /// Получить ссылку на иконку ярлыка
        /// </summary>
        /// <param name="shortcutPath">Путь к ярлыку</param>
        public string GetIconLocation(string shortcutPath);
    }
}
