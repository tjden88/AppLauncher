using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AppLauncher.Models
{
    /// <summary>
    /// Модель группы с ярлыками
    /// </summary>
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<ShortcutCell> Cells { get; set; } = new List<ShortcutCell>();
    }
}
