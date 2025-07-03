using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Converters
{
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                return level switch
                {
                    LogLevel.Debug => Brushes.Gray,
                    LogLevel.Info => Brushes.White,
                    LogLevel.Warning => Brushes.Yellow,
                    LogLevel.Error => Brushes.Red,
                    _ => Brushes.White
                };
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 