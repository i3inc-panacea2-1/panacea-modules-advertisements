using System;
using System.Globalization;
using System.Windows.Data;

namespace Panacea.Modules.Advertisements.Converters
{
    class HeightToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 5.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
