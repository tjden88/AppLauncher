// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Windows;

namespace AppLauncher.TestConsole
{
    public class Program
    {
        // Import the library containing PostMessage
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_COMMAND = 0x0112;      // Code for Windows command
        public const int WM_CLOSE = 0xF060;		 // Command code for close window
        [STAThread]
        public static void Main(string[] attr)
        {
            Console.WriteLine("Hello");
            var result = Process.GetProcessesByName("AppLauncher").First();
            Console.WriteLine(result);

            new Program().run();
        }

        void run()
        {
            Task.Run(() => producer());

            Console.WriteLine("Press <ENTER> to stop.");
            Console.ReadLine();
        }

        static void producer()
        {
            using (var mmf = MemoryMappedFile.CreateOrOpen("MyMapName", 1024))
            using (var view = mmf.CreateViewStream())
            {
                var writer = new BinaryWriter(view);
                var signal = new EventWaitHandle(false, EventResetMode.AutoReset, "MyEventName");
                var mutex = new Mutex(false, "MyMutex");

                for (int i = 0; i < 100; ++i)
                {
                    string message = "Message #" + i;

                    mutex.WaitOne();
                    writer.BaseStream.Position = 0;
                    writer.Write(message);
                    signal.Set();
                    mutex.ReleaseMutex();

                    Thread.Sleep(10000);
                }
            }
        }

       
    }



}
