using System.Windows.Media;

namespace AppLauncher.Services.Interfaces
{
    /// <summary>
    /// Получить изображение ярлыка
    /// </summary>
    public interface IIconBuilder
    {
        /// <summary> Получить изображение из файла / папки </summary>
        public ImageSource GetImage(string Path, int Index);
    }
}
