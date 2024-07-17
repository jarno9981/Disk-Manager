using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiskManager.Helpers
{
    public class DiskManagerFormatter
    {
        public async Task<bool> FormatDriveAsync(string driveLetter, string fileSystem, Action<string> progressCallback)
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

                // Format the drive using the specified file system
                await RunDiskPartCommand(driveLetter, fileSystem);

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

        private Task RunDiskPartCommand(string driveLetter, string fileSystem)
        {
            string formatCommand;
            switch (fileSystem.ToLower())
            {
                case "ntfs":
                    formatCommand = "format fs=ntfs quick";
                    break;
                case "fat":
                    formatCommand = "format fs=fat quick";
                    break;
                case "fat32":
                    formatCommand = "format fs=fat32 quick";
                    break;
                case "refs":
                    formatCommand = "format fs=refs";
                    break;
                default:
                    throw new ArgumentException($"Unsupported file system: {fileSystem}");
            }

            // Example: Use Process.Start to run DiskPart command
            var startInfo = new ProcessStartInfo
            {
                FileName = "diskpart.exe",
                Arguments = $"/s {GenerateScriptFile(driveLetter, formatCommand)}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();

            return Task.CompletedTask;
        }

        private string GenerateScriptFile(string driveLetter, string formatCommand)
        {
            string scriptContent = $"select volume {driveLetter}\n" +
                                   $"{formatCommand}\n" +
                                   $"assign letter={driveLetter}\n" +
                                   "exit\n";

            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, scriptContent);

            return tempFile;
        }
    }
}
