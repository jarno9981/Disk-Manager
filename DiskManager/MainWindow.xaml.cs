using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiskManager.Helpers;

namespace DiskManager
{
    public sealed partial class MainWindow : Window
    {
        private DiskManagerController driveCleaner;
        private DiskManagerFormatter diskManagerController;
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;

        public MainWindow()
        {
            InitializeComponent();
            DriveComboBox.ItemsSource = DriveInfo.GetDrives().Select(d => d.Name);
            driveCleaner = new DiskManagerController();
            diskManagerController = new DiskManagerFormatter();
            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(500, 500, 600, 675));
            this.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
            this.AppWindow.MoveInZOrderAtTop();
            this.AppWindow.ShowOnceWithRequestedStartupState();
            title();
        }

        public void title()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            appWindow = AppWindow.GetFromWindowId(windowId);

            if (!AppWindowTitleBar.IsCustomizationSupported())
            {
                // Why? Because I don't care
                throw new Exception("Unsupported OS version.");
            }
            else
            {
                titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = false;
            }
        }

        private async void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            if (DriveComboBox.SelectedItem == null)
            {
                await ShowContentDialog("Please select a drive.");
                return;
            }

            string driveLetter = DriveComboBox.SelectedItem.ToString();

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

        private async void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            if (DriveComboBox.SelectedItem == null)
            {
                await ShowContentDialog("Please select a drive.");
                return;
            }

            string driveLetter = DriveComboBox.SelectedItem.ToString();

            AddProgressMessage("Formatting drive...");
            bool result = await diskManagerController.FormatDriveAsync(driveLetter, UpdateProgress);

            if (result)
            {
                await ShowContentDialog("Drive formatted successfully.");
            }
            else
            {
                await ShowContentDialog("Failed to format the drive.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void DriveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DriveComboBox.SelectedItem != null)
            {
                string selectedDrive = DriveComboBox.SelectedItem.ToString();
                FormatButton.IsEnabled = !selectedDrive.Equals("C:\\", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
