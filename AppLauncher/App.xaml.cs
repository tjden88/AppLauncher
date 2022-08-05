using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AppLauncher.Infrastructure.Helpers;
using AppLauncher.Services;
using AppLauncher.ViewModels;
using AppLauncher.Views;

namespace AppLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public const string AppVersion = "1.0";

        /// <summary> Вьюмодель главного окна </summary>
        public static MainWindowViewModel MainWindowViewModel { get; } = new() {IsHidden = true};

        public static SettingsWindowViewModel SettingsWindowViewModel { get; } = new();

        public static ShortcutManager ShortcutManager { get; } = new(new IconBuilder(), new ShortcutBuilder());

        public static DataManager DataManager { get; } = new();

        public static Window ActiveWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);

        protected override void OnExit(ExitEventArgs e)
        {
            ShortcutManager.CleanNotUsedShortcuts();
            SettingsWindowViewModel.SaveData();
            base.OnExit(e);
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var wnd = new MainWindow();
            if (e.Args.Length > 0 && e.Args.Contains("-hide"))
            {
                wnd.Show();
            }
            else
            {
                wnd.Show();
                MainWindowViewModel.IsHidden = false;
            }

            SingleInstance();

            Task.Run(ListenCommands);

        }

        #region Constants and Fields

        /// <summary>The event mutex name.</summary>
        private const string UniqueEventName = "{ef2a6127-2705-4195-8069-aa726f8b6fca}";

        /// <summary>The unique mutex name.</summary>
        private const string UniqueMutexName = "{ef2a6127-2705-4195-8069-aa726f8b6fcb}";

        /// <summary>The event wait handle.</summary>
        private EventWaitHandle _EventWaitHandle;

        /// <summary>The mutex.</summary>
        private Mutex _Mutex;

        #endregion

        #region Methods

        private void SingleInstance()
        {
            _Mutex = new Mutex(true, UniqueMutexName, out var isOwned);
            _EventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            // So, R# would not give a warning that this variable is not used.
            GC.KeepAlive(_Mutex);

            if (isOwned)
            {
                // Spawn a thread which will be waiting for our event
                var thread = new Thread(
                    () =>
                    {
                        while (_EventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() => MainWindowViewModel.IsHidden = false));
                        }
                    })
                {
                    // It is important mark it as background otherwise it will prevent app from exiting.
                    IsBackground = true
                };

                thread.Start();
                return;
            }

            // Notify other instance so it could bring itself to foreground.
            _EventWaitHandle.Set();

            // Terminate this instance.
            Shutdown();
        }

        static void ListenCommands()
        {
            using var mmf = MemoryMappedFile.CreateOrOpen("AppLauncherMap", 1024);
            using var view = mmf.CreateViewStream();
            var reader = new BinaryReader(view);
            var signal = new EventWaitHandle(false, EventResetMode.AutoReset, "ShowAppEvent");
            var mutex = new Mutex(false, "AppLauncherMutex");

            while (true)
            {
                signal.WaitOne();
                mutex.WaitOne();
                reader.BaseStream.Position = 0;
                var message = reader.ReadString();
                if (message == "Activate")
                    MainWindowViewModel.IsHidden = !MainWindowViewModel.IsHidden;
                mutex.ReleaseMutex();
            }
        }

        #endregion

    }
}
