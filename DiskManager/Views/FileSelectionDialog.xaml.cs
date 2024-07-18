using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace DiskManager.Views;
public sealed partial class FileSelectionDialog : ContentDialog
{
    public List<FileItem> Files { get; private set; }

    public FileSelectionDialog(string[] filePaths)
    {
        this.InitializeComponent();
        this.Files = filePaths.Select(f => new FileItem { Name = f, IsSelected = false }).ToList();
        FileListView.ItemsSource = this.Files;
    }

    private void FileSelectionDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Handle deletion of selected files here
        List<string> selectedFiles = Files.Where(f => f.IsSelected).Select(f => f.Name).ToList();
        // Implement deletion logic here based on selectedFiles
        // For demonstration, you might want to display a confirmation or delete directly
    }
}

public class FileItem
{
    public string Name { get; set; }
    public bool IsSelected { get; set; }
}
