using System;
using System.Diagnostics;
using System.IO;
using AppLauncher.Models;
using AppLauncher.Services.Interfaces;
using WindowsShortcutFactory;

namespace AppLauncher.Services
{
    public class ShortcutBuilder : IShortcutBuilder
    {
        public bool CreateShortcut(string PathFrom, string SaveTo)
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

        public string GetExecutingPath(string shortcutPath)
        {
            throw new NotImplementedException();
        }

        public string GetIconLocation(string shortcutPath)
        {
            throw new NotImplementedException();
        }
    }
}
