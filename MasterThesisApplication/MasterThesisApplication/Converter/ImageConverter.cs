using MasterThesisApplication.Model.Utility;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MasterThesisApplication.Converter
{
    class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException("Target type must be System.Windows.Media.ImageSource.");
            try
            {
                if (value != null)
                {
                    string path = @"C:\Users\Kamil\Documents\Praca-Magisterska\MasterThesisApplication\GestureDatabase\" +
                                  value.ToString().Split('-')[0] + "\\" + value;
                    var bitmap = (Bitmap)Image.FromFile(path);
                    return bitmap.ToBitmapImage();
                }

                return null;
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
