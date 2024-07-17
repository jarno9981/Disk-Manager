using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DiskManager.Helpers
{
    public class DriveIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DiskInfo diskInfo)
            {
                if (diskInfo.Name == "C:\\" || diskInfo.DisplayName.Contains("C:\\"))
                {
                    return new BitmapImage(new Uri("ms-appx:///Assets/cdrive.png"));
                }
                else
                {
                    return new BitmapImage(new Uri("ms-appx:///Assets/drive.png"));
                }
            }

            return new BitmapImage(new Uri("ms-appx:///Assets/drive.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
