using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskManager.Helpers;
public class DiskPartitionInfo
{
    public string DriveLetter { get; set; }
    public string PartitionName { get; set; }
    public string FileSystem { get; set; }
    public string TotalSize { get; set; }
    public string FreeSpace { get; set; }
}
