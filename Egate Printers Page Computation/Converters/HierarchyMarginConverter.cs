﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Egate_Printers_Page_Computation.Converters
{
    [ValueConversion(typeof(int), typeof(Thickness))]
    public class HierarchyMarginConverter : IValueConverter
    {
        public double Multiplier { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int num = System.Convert.ToInt32(value);
            return new Thickness(num * Multiplier, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
