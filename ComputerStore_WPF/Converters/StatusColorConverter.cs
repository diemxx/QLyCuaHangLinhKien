using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ComputerStore_WPF.Converters
{
   
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                if (quantity < 5) return new SolidColorBrush(Color.FromRgb(231, 76, 60));   
                if (quantity <= 10) return new SolidColorBrush(Color.FromRgb(243, 156, 18)); 
                return new SolidColorBrush(Color.FromRgb(46, 204, 113));                     
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
