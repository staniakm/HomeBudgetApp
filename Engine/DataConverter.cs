using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Engine
{
    
    [ValueConversion(typeof(int), typeof(string))]
    //konwersja defaultowej daty na pusty string
    public class DataConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Console.WriteLine(value);
            DateTime original = (DateTime)value;
            // Console.WriteLine(original.ToString("yyyy-MM-dd"));
            if (original.ToString("yyyy-MM-dd") == "0001-01-01")
            {
                //   Console.WriteLine("podmiana");
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
