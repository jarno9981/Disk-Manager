using Microsoft.UI.Xaml.Media;

namespace DiskManager.Helpers
{
    public class DiskInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FileSystem { get; set; } // Add this line
        public string StorageGB { get; set; }
        public ImageSource Icon { get; set; }

    }
}
