using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace DiskManager.Helpers
{
    public class DiskManagerController
    {
        public async Task<bool> CleanDriveAsync(string driveLetter, bool cleanWindowsTemp, bool cleanUserTemp, bool cleanRecycleBin, Action<string> updateProgress)
        {
            try
            {
                if (cleanWindowsTemp)
                {
                    await RunHiddenCommandAsync($"del /q/f/s %windir%\\Temp\\*.*", updateProgress);
                }

                if (cleanUserTemp)
                {
                    await RunHiddenCommandAsync($"del /q/f/s %temp%\\*.*", updateProgress);
                }

                if (cleanRecycleBin)
                {
                    await RunHiddenCommandAsync($"rd /s /q {driveLetter}\\$Recycle.Bin", updateProgress);
                }

                return true;
            }
            catch (Exception ex)
            {
                updateProgress($"Error: {ex.Message}");
                return false;
            }
        }

        private Task RunHiddenCommandAsync(string command, Action<string> updateProgress)
        {
            return Task.Run(() =>
            {
                var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas"
                };

                using (var process = Process.Start(processInfo))
                {
                    process.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) updateProgress(e.Data); };
                    process.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) updateProgress(e.Data); };
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
            });
        }
    }
}
