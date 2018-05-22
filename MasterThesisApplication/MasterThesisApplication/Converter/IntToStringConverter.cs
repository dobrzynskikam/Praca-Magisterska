﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace MasterThesisApplication.Converter
{
    class IntToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Int16.Parse((string) value ?? throw new InvalidOperationException());
        }
    }
}