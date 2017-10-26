using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedString;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows;

namespace iAMPUpdate
{
    public sealed class ValueConverter
    {
        public static ConnectionToStrConverter StateConverter { get { return SingleTon<ConnectionToStrConverter>.GetInstance(); } }
        public static EnabledConverter ControlsEnabledConverter { get { return SingleTon<EnabledConverter>.GetInstance(); } }
        public static EnabledConverter2 ControlsEnabledConverter2 { get { return SingleTon<EnabledConverter2>.GetInstance(); } }
        public static VisibilityConverter ControlVisibilityConverter { get{ return SingleTon<VisibilityConverter>.GetInstance(); } }
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

    public sealed class EnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class EnabledConverter2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)values[0] && !(bool)values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(bool)value)
            {
                return new SolidColorBrush(Colors.Gray);
            }
            else
            {
                return new SolidColorBrush(Colors.Green);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
