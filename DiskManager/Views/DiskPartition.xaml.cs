using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.UI.Xaml;
using DiskManager.Helpers;

namespace DiskManager.Views
{
    public sealed partial class DiskPartition : Page
    {
        private ObservableCollection<DiskPartitionInfo> partitionInfoList;

        public DiskPartition()
        {
            this.InitializeComponent();
            partitionInfoList = new ObservableCollection<DiskPartitionInfo>();

            // Load drives initially
            LoadDrives();
        }

        private void LoadDrives()
        {
            // Clear previous entries
            partitionInfoList.Clear();

            // Get all drives
            var drives = DriveInfo.GetDrives();

            // Add drives information to partitionInfoList
            foreach (var drive in drives)
            {
                if (drive.IsReady)
                {
                    // Add main drive
                    partitionInfoList.Add(new DiskPartitionInfo
                    {
                        DriveLetter = drive.Name,
                        PartitionName = drive.VolumeLabel,
                        FileSystem = drive.DriveFormat,
                        TotalSize = FormatBytes(drive.TotalSize),
                        FreeSpace = FormatBytes(drive.TotalFreeSpace)
                    });
                }
            }

            // Bind partitionInfoList to ListView
            PartitionListView.ItemsSource = partitionInfoList;
        }

        private string FormatBytes(long bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB", "PB" };
            int index = 0;
            double size = bytes;
            while (size >= 1024 && index < suffix.Length - 1)
            {
                size /= 1024;
                index++;
            }
            return $"{size:0.##} {suffix[index]}";
        }

        private void FormatMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            var diskInfo = menuItem?.DataContext as DiskPartitionInfo;
            if (diskInfo != null)
            {
                // Placeholder for format logic
                string driveLetter = diskInfo.DriveLetter;
                // Example: FormatDrive(driveLetter);
                UpdateMenuFlyoutText($"Format selected drive: {driveLetter}");
            }
        }

        private void PartitionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            var diskInfo = menuItem?.DataContext as DiskPartitionInfo;
            if (diskInfo != null)
            {
                // Placeholder for partition logic
                string driveLetter = diskInfo.DriveLetter;
                // Example: PartitionDrive(driveLetter);
                UpdateMenuFlyoutText($"Partition selected drive: {driveLetter}");
            }
        }

        private void RemovePartitionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            var diskInfo = menuItem?.DataContext as DiskPartitionInfo;
            if (diskInfo != null)
            {
                // Placeholder for remove partition logic
                string driveLetter = diskInfo.DriveLetter;
                // Example: RemovePartition(driveLetter);
                UpdateMenuFlyoutText($"Remove partition for drive: {driveLetter}");
            }
        }

        private void UpdateMenuFlyoutText(string newText)
        {
            FormatMenuItem.Text = newText;
            PartitionMenuItem.Text = newText;
            RemovePartitionMenuItem.Text = newText;
        }

     
    }
}
