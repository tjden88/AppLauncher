using System.Windows.Media;

namespace AppLauncher.Services.Interfaces
{
    /// <summary>
    /// Получить изображение ярлыка
    /// </summary>
    public interface IIconBuilder
    {
        /// <summary> Получить изображение из файла / папки </summary>
        public ImageSource GetImage(string Path);

        /// <summary> Извлечь изображение из файла по индкусу значка </summary>
        public ImageSource GetImageByIndex(string PathToFile, int IconIndex);
    }
}
