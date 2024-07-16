using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace DiskManager.Helpers
{
    public class DiskManagerFormatter
    {
        public async Task<bool> FormatDriveAsync(string driveLetter, Action<string> progressCallback)
        {
            try
            {
                // Validate drive letter format
                if (!Path.IsPathRooted(driveLetter))
                {
                    driveLetter += Path.VolumeSeparatorChar; // Ensure format is "C:"
                }

                // Check if the drive exists
                if (!DriveInfo.GetDrives().Any(d => string.Equals(d.Name, driveLetter, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException($"Drive '{driveLetter}' is not valid or accessible.");
                }

                // Format the drive (example: using DiskPart command line utility)
                await RunDiskPartCommand(driveLetter);

                progressCallback?.Invoke("Formatting completed.");
                return true; // Return true for success
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error formatting drive '{driveLetter}': {ex.Message}");
                progressCallback?.Invoke($"Error: {ex.Message}");
                return false; // Return false if an error occurs
            }
        }

        private Task RunDiskPartCommand(string driveLetter)
        {
            // Example: Use Process.Start to run DiskPart command
            var startInfo = new ProcessStartInfo
            {
                FileName = "diskpart.exe",
                Arguments = $"/s {GenerateScriptFile(driveLetter)}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();

            return Task.CompletedTask;
        }

        private string GenerateScriptFile(string driveLetter)
        {
            string scriptContent = $"select volume {driveLetter}\n" +
                                   $"format fs=ntfs quick\n" +
                                   $"assign letter={driveLetter}\n" +
                                   "exit\n";

            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, scriptContent);

            return tempFile;
        }
    }
}
