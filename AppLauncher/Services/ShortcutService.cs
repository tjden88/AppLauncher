using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AppLauncher.Models;
using WindowsShortcutFactory;

namespace AppLauncher.Services
{
    /// <summary>
    /// Сервис работы с ярлыками
    /// </summary>
    public class ShortcutService
    {
        private readonly string _ShortcutsPath = Path.Combine(Environment.CurrentDirectory, "Shortcuts");

        private string _ShortcutFullPath(string ShortcutPath) => Path.Combine(_ShortcutsPath, ShortcutPath);

        public ShortcutService()
        {
            Directory.CreateDirectory(_ShortcutsPath);
        }


        /// <summary>
        /// Получить путь до целевого объекта ярлыка
        /// </summary>
        /// <param name="ShortcutsPath">Путь ярлыка</param>
        /// <returns></returns>
        public string GetFilePath(string ShortcutsPath)
        {
            var path = _ShortcutFullPath(ShortcutsPath);

            if (!File.Exists(path)) return null;

            using var sc = WindowsShortcut.Load(path);

            return sc.Path;
        }


        /// <summary>
        /// Создать ярлык во внутренней папке
        /// </summary>
        /// <param name="FileName">Имя к оригинальному файлу/папке</param>
        /// <returns>Созданный ярлык, не привязанный к группе</returns>
        public Shortcut CreateShortcut(string FileName)
        {

            const string linkExtension = ".lnk";

            var fileNameNoExt = Path.GetFileNameWithoutExtension(FileName);

            var newFileName = Path.Combine(_ShortcutsPath, fileNameNoExt + linkExtension);

            var intCount = 1;
            while (File.Exists(newFileName))
            {
                newFileName = Path.Combine(_ShortcutsPath, $"{fileNameNoExt}({intCount++}){linkExtension}");
            }


            var cuccess = CreateShortcut(FileName, newFileName);

            if (!cuccess) return null;
            

            return new Shortcut
            {
                Path = Path.GetFileName(newFileName),
                Name = fileNameNoExt,
            };

        }

        /// <summary>
        /// Удалить ярлык из рабочей папки
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        public void DeleteShortcut(string ShortcutPath)
        {
            File.Delete(_ShortcutFullPath(ShortcutPath));
        }

        /// <summary>
        /// Запустить ярлык
        /// </summary>
        /// <param name="ShortcutPath">Путь ярлыка</param>
        public void StartProcess(string ShortcutPath)
        {
            var path = _ShortcutFullPath(ShortcutPath);

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }


        /// <summary>
        /// Получить значок из ярлыка
        /// </summary>
        /// <param name="ShortcutPath">Путь к ярлыку</param>
        /// <returns>null, если файл или папка не найдена</returns>
        public ImageSource GetIconFromShortcut(string ShortcutPath)
        {
            var path = _ShortcutFullPath(ShortcutPath);

            if (!File.Exists(path)) return null;

            using var sc = WindowsShortcut.Load(path);

            var pathToIconFile = sc.IconLocation.Path;
            if (string.IsNullOrEmpty(pathToIconFile))
                pathToIconFile = sc.Path;

            if (File.Exists(pathToIconFile) || Directory.Exists(pathToIconFile))
            {
                return GetImgFromFileOrFolder(pathToIconFile);
            }

            return null;
        }

        /// <summary> Очистка лишних ярлыков </summary>
        public void CleanNotUsedShortcuts()
        {

        }

        #region Private


        /// <summary>
        /// Создать ярлык для файла и сохранить на диске
        /// </summary>
        /// <param name="originalFileName">Файл, для которого необходим ярлык</param>
        /// <param name="saveFileName">Путь для сохранения (вместе с расширением .lnk)</param>
        /// <returns></returns>
        private bool CreateShortcut(string originalFileName, string saveFileName)
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

        #endregion

    }
}
