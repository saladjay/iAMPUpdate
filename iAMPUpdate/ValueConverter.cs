using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedString;
using System.Windows.Data;
using System.Globalization;

namespace iAMPUpdate
{
    public sealed class ValueConverter
    {
        public static ConnectionToStrConverter StateConverter { get { return SingleTon<ConnectionToStrConverter>.GetInstance(); } }
    }

    public sealed class ConnectionToStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Disconnect";
            else
                return "Connect";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
