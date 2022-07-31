﻿using System.Collections.Generic;

namespace AppLauncher.Models
{
    /// <summary>
    /// Модель группы с ярлыками
    /// </summary>
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Shortcut> Links { get; set; } = new List<Shortcut>();
    }
}
