using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            var extension = Path.GetExtension(PathFrom).ToLower();

            return extension switch
            {
                ".lnk" => MakeFromLnkFile(PathFrom, SaveTo),
                ".url" => MakeFromUrlFile(PathFrom, SaveTo),
                _ => MakeFromFile(PathFrom, SaveTo)
            };
        }

        public string GetExecutingPath(string shortcutPath)
        {
            if (!File.Exists(shortcutPath))
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


        #region MakeLink

        private bool MakeFromFile(string PathFrom, string SaveTo)
        {
            try
            {
                using var sc = new WindowsShortcut();
                sc.Path = PathFrom;
                sc.WorkingDirectory = Path.GetDirectoryName(PathFrom);
                sc.Save(SaveTo);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private bool MakeFromLnkFile(string PathFrom, string SaveTo)
        {
            try
            {
                File.Copy(PathFrom, SaveTo);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        private bool MakeFromUrlFile(string PathFrom, string SaveTo)
        {
            try
            {
                var read = File.ReadAllLines(PathFrom);
                var linkString = read.FirstOrDefault(str => str.StartsWith("URL="));
                if (linkString is not { Length: > 4 } findedLink)
                    return false;

                var iconString = read.FirstOrDefault(str => str.StartsWith("IconFile="));
                var iconIndex = read.FirstOrDefault(str => str.StartsWith("IconIndex="));

                iconString = iconString?[9..];
                var converted = int.TryParse(iconIndex?[10..], out var index);

                using var sc = new WindowsShortcut();
                sc.Path = findedLink;

                if (iconString != null && converted)
                    sc.IconLocation = new IconLocation(iconString, index);

                sc.Save(SaveTo);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
        #endregion
    }
}
