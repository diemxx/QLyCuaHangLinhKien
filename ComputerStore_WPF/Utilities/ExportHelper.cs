using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ComputerStore_WPF.Utilities
{
    public static class ExportHelper
    {
        /// <summary>
        /// Xuất dữ liệu ra file CSV
        /// </summary>
        public static void ExportToCsv<T>(List<T> data, string filePath, string[] headers)
        {
            var sb = new StringBuilder();
            // BOM UTF-8 để Excel hiển thị đúng tiếng Việt
            sb.AppendLine(string.Join(",", headers));

            foreach (var item in data)
            {
                var values = new List<string>();
                foreach (var prop in typeof(T).GetProperties())
                {
                    var value = prop.GetValue(item)?.ToString() ?? "";
                    // Escape comma trong CSV
                    if (value.Contains(",") || value.Contains("\""))
                        value = "\"" + value.Replace("\"", "\"\"") + "\"";
                    values.Add(value);
                }
                sb.AppendLine(string.Join(",", values));
            }

            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
        }

        /// <summary>
        /// Xuất dữ liệu ra file HTML (có thể in)
        /// </summary>
        public static void ExportToHtml<T>(List<T> data, string filePath, string title, string[] headers)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'/>");
            sb.AppendLine($"<title>{title}</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: 'Segoe UI', sans-serif; padding: 20px; }");
            sb.AppendLine("h1 { color: #2c3e50; text-align: center; }");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            sb.AppendLine("th { background: #3498db; color: white; padding: 10px; text-align: left; }");
            sb.AppendLine("td { padding: 8px; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("tr:nth-child(even) { background: #f2f2f2; }");
            sb.AppendLine(".footer { text-align: right; margin-top: 20px; color: #666; }");
            sb.AppendLine("@media print { body { padding: 0; } }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine($"<h1>{title}</h1>");
            sb.AppendLine($"<p>Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.AppendLine("<table><thead><tr>");

            foreach (var header in headers)
                sb.AppendLine($"<th>{header}</th>");

            sb.AppendLine("</tr></thead><tbody>");

            foreach (var item in data)
            {
                sb.AppendLine("<tr>");
                foreach (var prop in typeof(T).GetProperties())
                {
                    var value = prop.GetValue(item)?.ToString() ?? "";
                    sb.AppendLine($"<td>{value}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table>");
            sb.AppendLine($"<div class='footer'>Tổng: {data.Count} bản ghi</div>");
            sb.AppendLine("</body></html>");

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
