using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using DiskManager.Helpers;

namespace DiskManager.Views
{
    public sealed partial class CleanDrivePage : Page
    {
        private DiskManagerController driveCleaner;
        private string selectedDrive;

        public CleanDrivePage()
        {
            this.InitializeComponent();
            driveCleaner = new DiskManagerController();
            this.Loaded += CleanDrivePage_Loaded;
        }

        private void CleanDrivePage_Loaded(object sender, RoutedEventArgs e)
        {
            DriveListView.ItemsSource = DriveInfo.GetDrives().Select(d => new DiskInfo
            {
                DisplayName = d.Name,
                StorageGB = $"{(d.TotalSize - d.TotalFreeSpace) / (1024 * 1024 * 1024)} GB"
            }).ToList();
        }

        private async void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            if (DriveListView.SelectedItem == null)
            {
                await ShowContentDialog("Please select a drive.");
                return;
            }

            string driveLetter = ((DiskInfo)DriveListView.SelectedItem).DisplayName;

            bool cleanWindowsTemp = TempCheckBox.IsChecked == true;
            bool cleanUserTemp = UserTempCheckBox.IsChecked == true;
            bool cleanRecycleBin = RecycleBinCheckBox.IsChecked == true;

            AddProgressMessage("Cleaning drive...");
            bool result = await driveCleaner.CleanDriveAsync(driveLetter, cleanWindowsTemp, cleanUserTemp, cleanRecycleBin, UpdateProgress);

            if (result)
            {
                await ShowContentDialog("Drive cleaned successfully.");
            }
            else
            {
                await ShowContentDialog("Failed to clean the drive.");
            }
        }

        private void DriveListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DriveListView.SelectedItem != null)
            {
                selectedDrive = ((DiskInfo)DriveListView.SelectedItem).DisplayName;
            }
        }

        private void UpdateProgress(string progress)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                AddProgressMessage(progress);
            });
        }

        private void AddProgressMessage(string message)
        {
            ProgressListView.Items.Add(message);
        }

        private async Task ShowContentDialog(string message)
        {
            var dialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Information",
                Content = message,
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }
    }
}
