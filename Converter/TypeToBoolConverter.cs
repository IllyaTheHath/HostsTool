using System;
using System.Globalization;
using System.Windows.Data;

using HostsTool.Model;

namespace HostsTool.Converter
{
    public class TypeToBoolConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (SourceType)value == (SourceType)Int32.Parse(parameter.ToString());
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Boolean)value == true ? SourceType.Local : SourceType.Web;
        }
    }
}