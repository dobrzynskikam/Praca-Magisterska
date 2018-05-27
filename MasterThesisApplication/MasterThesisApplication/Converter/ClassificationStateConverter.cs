using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using MasterThesisApplication.Model;

namespace MasterThesisApplication.Converter
{
    class ClassificationStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FeatureState state = (FeatureState)value;
            switch (state)
            {
                case FeatureState.CorrectClassification:
                    return new SolidColorBrush(Colors.ForestGreen);
                case FeatureState.IncorrectClassification:
                    return new SolidColorBrush(Colors.Crimson);
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
