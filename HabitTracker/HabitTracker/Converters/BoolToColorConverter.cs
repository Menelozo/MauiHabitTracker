using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace HabitTracker.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Colors.MediumPurple : Colors.FloralWhite;
            }
            return Colors.FloralWhite;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color == Colors.FloralWhite;
            }
            return false;
        }
    }
}
