using System;
using System.Globalization;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace Egate_Printers_Page_Computation.Converters
{
    public class BitmapImageConverter : IValueConverter
    {
        public int DecodePixedWidth { get; set; }
        public int DecodePixelHeight { get; set; }

        public BitmapImageConverter()
        {
            DecodePixedWidth = -1;
            DecodePixelHeight = -1;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage img = null;
            string path = (string)value;
            if (File.Exists(path))
            {
                img = new BitmapImage();
                img.BeginInit();
                if (this.DecodePixedWidth >= 0) img.DecodePixelWidth = this.DecodePixedWidth;
                if (this.DecodePixelHeight >= 0) img.DecodePixelHeight = this.DecodePixelHeight;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = new Uri(path);
                try
                {
                    img.EndInit();
                }
                catch (FileFormatException)
                {
                    img = null;
                }
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
