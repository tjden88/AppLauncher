using System;
using System.Diagnostics;
using System.IO;
using AppLauncher.Services.Interfaces;
using WindowsShortcutFactory;

namespace AppLauncher.Services
{
    public class ShortcutBuilder : IShortcutBuilder
    {
        public bool CreateShortcut(string PathFrom, string SaveTo)
        {
            if (!File.Exists(PathFrom) && !Directory.Exists(PathFrom))
                throw new FileNotFoundException(PathFrom);

            try
            {
                using var sc = new WindowsShortcut();
                sc.Path = PathFrom;
                sc.WorkingDirectory = Path.GetDirectoryName(PathFrom);
                sc.IconLocation = new(PathFrom);
                sc.Save(SaveTo);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public string GetExecutingPath(string shortcutPath)
        {
            if (!File.Exists(shortcutPath) || Path.GetExtension(shortcutPath).ToLower() != ".lnk")
                return null;

            using var lnk = WindowsShortcut.Load(shortcutPath);
            return lnk.Path;
        }

        public (string, int) GetIconLocation(string shortcutPath)
        {
            if (!File.Exists(shortcutPath))
                return (null, 0);

            using var lnk = WindowsShortcut.Load(shortcutPath);

            var lnkIconLocation = lnk.IconLocation;
            return lnkIconLocation == null ? (null, 0) : (lnkIconLocation.Path, lnkIconLocation.Index);
        }
    }
}
