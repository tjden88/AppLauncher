
using System.Collections.Generic;

namespace AppLauncher.Models
{
    /// <summary>
    /// Группа ссылок
    /// </summary>
    public class ShortcutCell
    {
        public int Id { get; set; }

        public Shortcut BigLink { get; set; }

        public Shortcut Link1 { get; set; }

        public Shortcut Link2 { get; set; }

        public Shortcut Link3 { get; set; }

        public Shortcut Link4 { get; set; }

        public int GroupId { get; set; }


        public List<Shortcut> GetAllShortcuts()
        {
            var result = new List<Shortcut>();
            if (Link1 != null)
                result.Add(Link1);
            if (Link2 != null)
                result.Add(Link2);
            if (Link3 != null)
                result.Add(Link3);
            if (Link4 != null)
                result.Add(Link4);
            if (BigLink != null)
                result.Add(BigLink);

            return result;
        }
    }
}
