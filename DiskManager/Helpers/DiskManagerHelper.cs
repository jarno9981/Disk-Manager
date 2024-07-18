using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DiskManager.Helpers;
public class DiskManagerHelper
{
    public ObservableCollection<DiskInfo> GetDrives()
    {
        ObservableCollection<DiskInfo> drives = new ObservableCollection<DiskInfo>();

        // Get all storage devices
        var devices = KnownFolders.RemovableDevices.GetFoldersAsync().AsTask().Result;

        foreach (var device in devices)
        {
            drives.Add(new DiskInfo
            {
                DisplayName = device.Name,
                FileSystem = "Unknown" // You may need to further query for filesystem
            });
        }

        return drives;
    }
}
