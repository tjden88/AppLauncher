﻿namespace AppLauncher.Models
{
    /// <summary>
    /// Ярлык приложения
    /// </summary>
    public class Shortcut
    {
        /// <summary> Имя ярлыка </summary>
        public string Name { get; set; }

        /// <summary> Описание </summary>
        public string Description { get; set; }

        /// <summary> Путь для запуска </summary>
        public string ExecutingPath { get; set; }

        /// <summary> Данные изображения </summary>
        public byte[] Image { get; set; }

    }
}
