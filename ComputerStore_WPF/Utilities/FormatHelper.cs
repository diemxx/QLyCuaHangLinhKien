using System;
using System.Globalization;

namespace ComputerStore_WPF.Utilities
{
    public static class FormatHelper
    {
        /// <summary>
        /// Format số tiền thành chuỗi VNĐ (VD: 15,000,000 VNĐ)
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return string.Format(new CultureInfo("vi-VN"), "{0:#,##0} VNĐ", amount);
        }

        /// <summary>
        /// Format ngày tháng (VD: 15/03/2025)
        /// </summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Format ngày giờ (VD: 15/03/2025 10:30)
        /// </summary>
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }
    }
}
