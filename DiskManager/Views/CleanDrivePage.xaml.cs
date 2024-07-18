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
                StorageGB = $"{(d.TotalSize - d.TotalFreeSpace) / (1024 * 1024 * 1024)}",
                TotalStorageGB = $"{d.TotalSize / (1024 * 1024 * 1024)}"
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

            if (driveLetter.Equals("C:\\", StringComparison.OrdinalIgnoreCase))
            {
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
            else
            {
                await ShowFileSelectionDialog(driveLetter);
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

        private async Task ShowFileSelectionDialog(string driveLetter)
        {
            try
            {
                var files = Directory.GetFiles(driveLetter, "*", SearchOption.AllDirectories)
                                    .Where(file => !file.StartsWith(driveLetter + "\\"));
                FileSelectionDialog fileSelectionDialog = new FileSelectionDialog(files.ToArray());
                fileSelectionDialog.XamlRoot = this.Content.XamlRoot;
                await fileSelectionDialog.ShowAsync();
            }
            catch (UnauthorizedAccessException)
            {
                await ShowContentDialog($"Access denied to system folders on drive {driveLetter}.");
            }
            catch (Exception ex)
            {
                await ShowContentDialog($"Error: {ex.Message}");
            }
        }

        private Task DeleteSelectedFilesAsync(List<string> selectedFiles)
        {
            return Task.Run(() =>
            {
                foreach (var file in selectedFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        AddProgressMessage($"Error deleting {file}: {ex.Message}");
                    }
                }
            });
        }

        private void FileSelectionDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle primary button click if necessary
        }
    }
}
