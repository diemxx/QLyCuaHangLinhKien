using System;
using System.Globalization;
using System.Windows.Data;

namespace ComputerStore_WPF.Converters
{
    
    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "0 VNĐ";
            decimal amount;
            if (decimal.TryParse(value.ToString(), out amount))
                return string.Format("{0:#,##0} VNĐ", amount);
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0m;
            string str = value.ToString().Replace("VNĐ", "").Replace(",", "").Replace(" ", "").Trim();
            decimal result;
            if (decimal.TryParse(str, out result))
                return result;
            return 0m;
        }
    }
}
