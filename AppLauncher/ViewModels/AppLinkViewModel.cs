using System;
using System.Diagnostics;
using System.IO;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace AppLauncher.ViewModels
{
    /// <summary>
    /// Модель - представление ярлыка для запуска
    /// </summary>
    public class AppLinkViewModel : ViewModel
    {

        #region Name : string - Имя ярлыка

        /// <summary>Имя ярлыка</summary>
        private string _Name;

        /// <summary>Имя ярлыка</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion

        #region FilePath : string - Путь для запуска

        /// <summary>Путь для запуска</summary>
        private string _FilePath;

        /// <summary>Путь для запуска</summary>
        public string FilePath
        {
            get => _FilePath;
            set => Set(ref _FilePath, value);
        }

        #endregion

        #region Command LaunchCommand - Запуск

        /// <summary>Запуск</summary>
        private Command _LaunchCommand;

        /// <summary>Запуск</summary>
        public Command LaunchCommand => _LaunchCommand
            ??= new Command(OnLaunchCommandExecuted, CanLaunchCommandExecute, "Запуск");

        /// <summary>Проверка возможности выполнения - Запуск</summary>
        private bool CanLaunchCommandExecute() => true;

        /// <summary>Логика выполнения - Запуск</summary>
        private void OnLaunchCommandExecuted()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = FilePath,
                UseShellExecute = true
            });
        }

        #endregion

        /// <summary> Создать вьюмодель из ссылки на файл / ярлык / папку </summary>
        public static AppLinkViewModel CreateLinkViewModelFromLink(string Url)
        {
            var name = Path.GetFileName(Url);
            var extension = Path.GetExtension(Url);

            var path = extension switch
            {
                "lnk" => GetShortcutTarget(Url),
                _ => Url
            };
            return new AppLinkViewModel
            {
                FilePath = path,
                Name = name,
            };


        }



        private static string GetShortcutTarget(string file)
        {
            try
            {
                if (System.IO.Path.GetExtension(file).ToLower() != ".lnk")
                {
                    throw new Exception("Supplied file must be a .LNK file");
                }

                var fileStream = File.Open(file, FileMode.Open, FileAccess.Read);
                using var fileReader = new BinaryReader(fileStream);
                fileStream.Seek(0x14, SeekOrigin.Begin);     // Seek to flags
                var flags = fileReader.ReadUInt32();        // Read flags
                if ((flags & 1) == 1)
                {                      // Bit 1 set means we have to
                    // skip the shell item ID list
                    fileStream.Seek(0x4c, SeekOrigin.Begin); // Seek to the end of the header
                    uint offset = fileReader.ReadUInt16();   // Read the length of the Shell item ID list
                    fileStream.Seek(offset, SeekOrigin.Current); // Seek past it (to the file locator info)
                }

                var fileInfoStartsAt = fileStream.Position; // Store the offset where the file info
                // structure begins
                var totalStructLength = fileReader.ReadUInt32(); // read the length of the whole struct
                fileStream.Seek(0xc, SeekOrigin.Current); // seek to offset to base pathname
                var fileOffset = fileReader.ReadUInt32(); // read offset to base pathname
                // the offset is from the beginning of the file info struct (fileInfoStartsAt)
                fileStream.Seek((fileInfoStartsAt + fileOffset), SeekOrigin.Begin); // Seek to beginning of
                // base pathname (target)
                var pathLength = (totalStructLength + fileInfoStartsAt) - fileStream.Position - 2; // read
                // the base pathname. I don't need the 2 terminating nulls.
                var linkTarget = fileReader.ReadChars((int)pathLength); // should be unicode safe
                var link = new string(linkTarget);

                var begin = link.IndexOf("\0\0", StringComparison.Ordinal);
                if (begin > -1)
                {
                    var end = link.IndexOf("\\\\", begin + 2, StringComparison.Ordinal) + 2;
                    end = link.IndexOf('\0', end) + 1;

                    var firstPart = link[..begin];
                    var secondPart = link[end..];

                    return firstPart + secondPart;
                }
                else
                {
                    return link;
                }
            }
            catch
            {
                return "";
            }
        }

    }
}
