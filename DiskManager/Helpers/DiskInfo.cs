using Microsoft.UI.Xaml.Media;

namespace DiskManager.Helpers;
public class DiskInfo
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string FileSystem { get; set; }
    public string StorageGB { get; set; }
    public string TotalStorageGB { get; set; } // Add this line
    public ImageSource Icon { get; set; }

    // Add a property to display "used/total GB"
    public string StorageDisplay => $"{StorageGB} / {TotalStorageGB} GB";
}
