using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GestureApplication.Converters
{
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value!=null)
            {
                Bitmap bi = null;
                var imageSourceConverter = new ImageSourceConverter();
                using (var memoryStream = new MemoryStream())
                {
                    bi.Save(memoryStream, ImageFormat.Png);
                    var snapshotBytes = memoryStream.ToArray();
                    return (ImageSource)imageSourceConverter.ConvertFrom(snapshotBytes);
                }
            }
            //BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //MemoryStream ms = new MemoryStream();
            //Bitmap bitmap = value as Bitmap;
            //bitmap.Save(ms, ImageFormat.Bmp);
            //ms.Seek(0, SeekOrigin.Begin);
            //bi.StreamSource = ms;
            //bi.EndInit();
            //return bi;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
