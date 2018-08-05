using System;
using System.Globalization;
using System.Windows.Data;

namespace HostsTool.Converter
{
    class TypeToRadioConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Int32)value == Int32.Parse(parameter.ToString()) ? true : false;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return Int32.Parse(parameter.ToString());
        }
    }
}
