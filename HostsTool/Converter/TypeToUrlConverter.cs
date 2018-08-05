using HostsTool.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HostsTool.Converter
{
    class TypeToUrlConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Int32)value == SourceTypes.Local ? false : true;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Boolean)value == true ? SourceTypes.Web : SourceTypes.Local;
        }
    }
}
