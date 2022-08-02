
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;


var processIsRun = Process.GetProcessesByName("AppLauncher").Any();

if (!processIsRun)
{
    Process.Start(new ProcessStartInfo()
    {
        FileName = Path.Combine(Environment.CurrentDirectory, "AppLauncher.exe"),
        UseShellExecute = true
    });
};


using var mmf = MemoryMappedFile.CreateOrOpen("AppLauncherMap", 1024);
using var view = mmf.CreateViewStream();
var writer = new BinaryWriter(view);
var signal = new EventWaitHandle(false, EventResetMode.AutoReset, "ShowAppEvent");
var mutex = new Mutex(false, "AppLauncherMutex");

var message = "Activate";

mutex.WaitOne();
writer.BaseStream.Position = 0;
writer.Write(message);
signal.Set();
mutex.ReleaseMutex();
