using System;
using AppLauncher.Models;
using AppLauncher.Services.Interfaces;

namespace AppLauncher.Services
{
    public class LnkShortcutManager : IShortcutManager
    {
        public Shortcut CreateShortcut(string Path)
        {
            throw new NotImplementedException();
        }
    }

    public class UrlShortcutManager : IShortcutManager
    {
        public Shortcut CreateShortcut(string Path)
        {
            throw new NotImplementedException();
        }
    }

    public class FileFolderShortcutManager : IShortcutManager
    {
        public Shortcut CreateShortcut(string Path)
        {
            throw new NotImplementedException();
        }
    }
}
