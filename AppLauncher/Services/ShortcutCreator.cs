using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsShortcutFactory;

namespace AppLauncher.Services
{
    /// <summary>
    /// Создание ярлыков
    /// </summary>
    public class ShortcutCreator
    {

        /// <summary>
        /// Создать ярлык для файла и сохранить на диске
        /// </summary>
        /// <param name="originalFileName">Файл, для которого необходим ярлык</param>
        /// <param name="saveFileName">Путь для сохранения (вместе с расширением .lnk)</param>
        /// <returns></returns>
        public bool CreateShortcut(string originalFileName, string saveFileName)
        {
            if (!File.Exists(originalFileName) && !Directory.Exists(originalFileName))
                throw new FileNotFoundException(originalFileName);

            try
            {
                if (Path.GetExtension(originalFileName) == ".lnk")
                    File.Copy(originalFileName, saveFileName);
                else
                {
                    using var sc = new WindowsShortcut();
                    sc.Path = originalFileName;
                    sc.WorkingDirectory = Path.GetDirectoryName(originalFileName);
                    sc.Save(saveFileName);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

        }


        /// <summary>
        /// Получить значок из ярлыка
        /// </summary>
        /// <param name="ShortcutFileName">Путь к ярлыку</param>
        public ImageSource GetIconFromShortcut(string ShortcutFileName)
        {

            using var sc = WindowsShortcut.Load(ShortcutFileName);

            var pathToIconFile = sc.IconLocation.Path;
            if (string.IsNullOrEmpty(pathToIconFile))
                pathToIconFile = sc.Path;

            Icon icon = null;

            if (pathToIconFile != null && File.Exists(pathToIconFile)) 
                icon = Icon.ExtractAssociatedIcon(pathToIconFile);

            icon ??= new Icon(SystemIcons.Application, 128, 128);

            return ToImageSource(icon);
        }


        private ImageSource ToImageSource(Icon icon) =>
            Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
    }
}
