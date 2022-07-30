using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsShortcutFactory;

namespace AppLauncher.Services
{
    /// <summary>
    /// Сервис работы с ярлыками
    /// </summary>
    public class ShortcutService
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
        /// <returns>null, если файл или папка не найдена</returns>
        public ImageSource GetIconFromShortcut(string ShortcutFileName)
        {

            if (!File.Exists(ShortcutFileName) && !Directory.Exists(ShortcutFileName))
            {
                return null;
            }

            using var sc = WindowsShortcut.Load(ShortcutFileName);

            var pathToIconFile = sc.IconLocation.Path;
            if (string.IsNullOrEmpty(pathToIconFile))
                pathToIconFile = sc.Path;

            if (File.Exists(pathToIconFile) || Directory.Exists(pathToIconFile))
            {
                return GetImgFromFileOrFolder(pathToIconFile);
            }

            return null;
        }


        private ImageSource GetImgFromFileOrFolder(string Path)
        {
            var shinfo = new SHFILEINFO();

            //Call function with the path to the folder you want the icon for
            SHGetFileInfo(Path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_LARGEICON);

            using var icon = Icon.FromHandle(shinfo.hIcon);

            //Convert icon to a Bitmap source
            ImageSource img = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                new Int32Rect(0, 0, icon.Width, icon.Height),
                BitmapSizeOptions.FromEmptyOptions());

            return img;
        }


        #region Interop

        //Struct used by SHGetFileInfo function
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        //Constants flags for SHGetFileInfo 
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon

        //Import SHGetFileInfo functio
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        #endregion


    }
}
