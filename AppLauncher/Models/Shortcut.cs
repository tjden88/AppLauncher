namespace AppLauncher.Models
{
    /// <summary>
    /// Ярлык приложения
    /// </summary>
    public class Shortcut
    {
        /// <summary> Имя ярлыка </summary>
        public string Name { get; set; }

        /// <summary> Путь для запуска </summary>
        public string Path { get; set; }

        /// <summary> Сначала попытаться определить иконку из ярлыка </summary>
        public bool IsDefineIconFromShortcut { get; set; }

    }
}
