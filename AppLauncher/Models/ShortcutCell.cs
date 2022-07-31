
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

    }
}
