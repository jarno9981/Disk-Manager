using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using DiskManager.Helpers;

namespace DiskManager.Views
{
    public sealed partial class FormatDrivePage : Page
    {
        private DiskManagerFormatter diskManagerController;
        private string selectedDrive;
        private bool isPageLoaded = false;

        public FormatDrivePage()
        {
            this.InitializeComponent();
            diskManagerController = new DiskManagerFormatter();
            this.Loaded += FormatDrivePage_Loaded;
        }

        private void FormatDrivePage_Loaded(object sender, RoutedEventArgs e)
        {
            // Load drives only when the page is fully loaded
            DriveListView.ItemsSource = DriveInfo.GetDrives().Select(d => new DiskInfo { DisplayName = d.Name, FileSystem = d.DriveFormat });
            isPageLoaded = true;
        }

        private async void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPageLoaded)
            {
                return; // Prevent operation if page is not fully loaded
            }

            if (string.IsNullOrEmpty(selectedDrive))
            {
                await ShowContentDialog("Please select a drive.");
                return;
            }

            ComboBoxItem selectedItem = (ComboBoxItem)FileSystemComboBox.SelectedItem;
            string fileSystem = selectedItem.Content.ToString();

            AddProgressMessage("Formatting drive...");
            bool result = await diskManagerController.FormatDriveAsync(selectedDrive, fileSystem, UpdateProgress);

            if (result)
            {
                await ShowContentDialog("Drive formatted successfully.");

                // Delay for 3 seconds
                await Task.Delay(3000);

                // Refresh the drive list
                RefreshDriveList();
            }
            else
            {
                await ShowContentDialog("Failed to format the drive.");
            }
        }

        //refresh data if success so you can see the filesystem type without leaving and opening it again 
        private void RefreshDriveList()
        {
            DriveListView.ItemsSource = DriveInfo.GetDrives().Select(d => new DiskInfo { DisplayName = d.Name, FileSystem = d.DriveFormat });
        }

        private void DriveListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isPageLoaded)
            {
                return; // Prevent operation if the page is not fully loaded
            }

            if (DriveListView.SelectedItem != null)
            {
                selectedDrive = ((DiskInfo)DriveListView.SelectedItem).DisplayName;
                UpdateFormatButtonState();

                // Check if the selected drive is C:\
                if (selectedDrive.Equals("C:\\", StringComparison.OrdinalIgnoreCase))
                {
                    FormatButton.IsEnabled = false;
                }
            }
        }


        private void FileSystemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isPageLoaded)
            {
                return; // Prevent operation if page is not fully loaded
            }

            UpdateFormatButtonState();
            UpdateWarningVisibility();
        }

        private void UpdateFormatButtonState()
        {
            // Disable the format button if no drive is selected or if the selected drive is C:\
            FormatButton.IsEnabled = !string.IsNullOrEmpty(selectedDrive) && !selectedDrive.Equals("C:\\", StringComparison.OrdinalIgnoreCase);
        }


        private void UpdateWarningVisibility()
        {
            ComboBoxItem selectedItem = (ComboBoxItem)FileSystemComboBox.SelectedItem;
            string selectedFileSystem = selectedItem.Content.ToString();

            if (selectedFileSystem == "ReFS")
            {
                WarningTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                WarningTextBlock.Visibility = Visibility.Collapsed;
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
