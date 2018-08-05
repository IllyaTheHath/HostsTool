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
    class TypeToContentConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Int32)value == SourceTypes.Local ? true : false;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            return (Boolean)value == true ? SourceTypes.Local : SourceTypes.Web;
        }
    }
}
