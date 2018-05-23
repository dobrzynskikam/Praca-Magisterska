using MasterThesisApplication.Model.Utility;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MasterThesisApplication.Converter
{
    class ImageConverter : IValueConverter
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string DatabasePath = Path.Combine(AssemblyPath.Replace("MasterThesisApplication\\bin\\Debug", ""), "GestureDatabase");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException("Target type must be System.Windows.Media.ImageSource.");
            try
            {
                if (value != null)
                {
                    string path = value.ToString();

                    if (Path.GetFileName(value.ToString()) == value)
                    {
                        path = Path.Combine(DatabasePath, value.ToString().Split('-')[0], value.ToString());
                        //path = DatabasePath + value.ToString().Split('-')[0] + "\\" + value;
                    }
                    
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
