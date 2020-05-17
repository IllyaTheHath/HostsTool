using System;
using System.Globalization;
using System.Windows.Data;

using HostsTool.Model;

namespace HostsTool.Converter
{
    public class TypeToKindConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (SourceType)value == SourceType.Local ? "Monitor" : "Web";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (String)value == "Monitor" ? SourceType.Local : SourceType.Web;
        }
    }
}