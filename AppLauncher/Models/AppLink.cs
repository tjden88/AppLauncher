namespace AppLauncher.Models
{
    /// <summary>
    /// Ярлык приложения
    /// </summary>
    public class AppLink
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public int GroupId { get; set; }

        public bool IsDirectory { get; set; }
    }
}
