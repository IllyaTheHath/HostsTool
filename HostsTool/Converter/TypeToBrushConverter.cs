using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using HostsTool.Data;

namespace HostsTool.Converter
{
    class TypeToBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Int32)value == SourceTypes.Local ? Brushes.Blue : Brushes.Purple;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Brush)value == Brushes.Blue ? SourceTypes.Local : SourceTypes.Web;
        }
    }
}
