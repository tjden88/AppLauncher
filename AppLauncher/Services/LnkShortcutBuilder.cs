using System;
using AppLauncher.Models;
using AppLauncher.Services.Interfaces;

namespace AppLauncher.Services
{
    public class LnkShortcutBuilder : IShortcutBuilder
    {
        public Shortcut CreateShortcut(string Path)
        {
            throw new NotImplementedException();
        }
    }

    public class UrlShortcutBuilder : IShortcutBuilder
    {
        public Shortcut CreateShortcut(string Path)
        {
            throw new NotImplementedException();
        }
    }

    public class FileFolderShortcutBuilder : IShortcutBuilder
    {
        public Shortcut CreateShortcut(string Path)
        {
            throw new NotImplementedException();
        }
    }
}
