using System;
using System.Globalization;
using System.Windows.Data;

namespace Engine
{

    [ValueConversion(typeof(int), typeof(string))]
    //konwersja defaultowej daty na pusty string
    public class DataConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime original = (DateTime)value;
            if (original.ToString("yyyy-MM-dd") == "0001-01-01")
            {
                return "";
            }
            return original.ToString("yyyy-MM-dd");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class BoolVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return (bool)value ? Visibility.Visible : Visibility.Collapsed;

    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class InvertBoolConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool)
    //        {
    //            return !(bool)value;
    //        }
    //        else return null;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
