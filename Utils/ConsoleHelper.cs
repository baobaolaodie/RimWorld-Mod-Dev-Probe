using System;
using System.Text;

namespace RimWorldModDevProbe
{
    /// <summary>
    /// 控制台辅助类 - 提供颜色输出和格式化方法
    /// </summary>
    public static class ConsoleHelper
    {
        public static readonly ConsoleColor DefaultColor = ConsoleColor.Gray;
        public static readonly ConsoleColor SuccessColor = ConsoleColor.Green;
        public static readonly ConsoleColor ErrorColor = ConsoleColor.Red;
        public static readonly ConsoleColor WarningColor = ConsoleColor.Yellow;
        public static readonly ConsoleColor InfoColor = ConsoleColor.Cyan;
        public static readonly ConsoleColor HighlightColor = ConsoleColor.White;
        public static readonly ConsoleColor DimColor = ConsoleColor.DarkGray;

        public static void WriteColor(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = originalColor;
        }

        public static void WriteLineColor(string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        public static void WriteSuccess(string message)
        {
            WriteLineColor($"✓ {message}", SuccessColor);
        }

        public static void WriteError(string message)
        {
            WriteLineColor($"✗ {message}", ErrorColor);
        }

        public static void WriteWarning(string message)
        {
            WriteLineColor($"⚠ {message}", WarningColor);
        }

        public static void WriteInfo(string message)
        {
            WriteLineColor($"ℹ {message}", InfoColor);
        }

        public static void WriteHighlight(string message)
        {
            WriteLineColor(message, HighlightColor);
        }

        public static void WriteDim(string message)
        {
            WriteLineColor(message, DimColor);
        }

        public static void WriteHeader(string title)
        {
            var width = Console.WindowWidth > 0 ? Console.WindowWidth : 80;
            var separator = new string('=', Math.Max(width - 1, 20));
            
            Console.WriteLine();
            WriteLineColor(separator, HighlightColor);
            WriteLineColor($"  {title}", HighlightColor);
            WriteLineColor(separator, HighlightColor);
            Console.WriteLine();
        }

        public static void WriteSection(string title)
        {
            Console.WriteLine();
            WriteLineColor($"--- {title} ---", InfoColor);
        }

        public static void WriteKeyValue(string key, string value, ConsoleColor valueColor = ConsoleColor.White)
        {
            WriteColor($"{key}: ", DefaultColor);
            WriteLineColor(value, valueColor);
        }

        public static void WriteListItem(string item, int indent = 0)
        {
            var indentStr = new string(' ', indent * 2);
            WriteLineColor($"{indentStr}• {item}", DefaultColor);
        }

        public static void WriteTable(string[] headers, string[][] rows)
        {
            if (headers == null || headers.Length == 0) return;

            var columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
            }

            foreach (var row in rows)
            {
                for (int i = 0; i < Math.Min(row.Length, headers.Length); i++)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], row[i]?.Length ?? 0);
                }
            }

            var headerLine = FormatTableRow(headers, columnWidths);
            WriteLineColor(headerLine, HighlightColor);

            var separatorLine = FormatTableSeparator(columnWidths);
            WriteLineColor(separatorLine, DimColor);

            foreach (var row in rows)
            {
                var rowLine = FormatTableRow(row, columnWidths);
                WriteLineColor(rowLine, DefaultColor);
            }
        }

        private static string FormatTableRow(string[] values, int[] widths)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < widths.Length; i++)
            {
                var value = i < values.Length ? values[i] ?? "" : "";
                sb.Append(value.PadRight(widths[i] + 2));
            }
            return sb.ToString().TrimEnd();
        }

        private static string FormatTableSeparator(int[] widths)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < widths.Length; i++)
            {
                sb.Append(new string('-', widths[i]) + "  ");
            }
            return sb.ToString().TrimEnd();
        }

        public static void WriteProgressBar(string label, int current, int total, int width = 30)
        {
            var percentage = total > 0 ? (double)current / total : 0;
            var filledWidth = (int)(percentage * width);
            var emptyWidth = width - filledWidth;

            var filledBar = new string('█', filledWidth);
            var emptyBar = new string('░', emptyWidth);

            Console.Write($"\r{label}: [");
            WriteColor(filledBar, SuccessColor);
            WriteColor(emptyBar, DimColor);
            Console.Write($"] {percentage:P0} ({current}/{total})");
        }

        public static void ClearProgressBar()
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth > 0 ? Console.WindowWidth - 1 : 79) + "\r");
        }

        public static void WriteBox(string content, ConsoleColor borderColor = ConsoleColor.White)
        {
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var maxLength = 0;
            foreach (var line in lines)
            {
                maxLength = Math.Max(maxLength, line.Length);
            }

            var horizontalBorder = "+" + new string('-', maxLength + 2) + "+";
            
            WriteLineColor(horizontalBorder, borderColor);
            foreach (var line in lines)
            {
                WriteLineColor($"| {line.PadRight(maxLength)} |", borderColor);
            }
            WriteLineColor(horizontalBorder, borderColor);
        }

        public static string Prompt(string message, string defaultValue = null)
        {
            WriteColor(message, InfoColor);
            if (!string.IsNullOrEmpty(defaultValue))
            {
                WriteColor($" [{defaultValue}]", DimColor);
            }
            WriteColor(": ", DefaultColor);

            var input = Console.ReadLine();
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }

        public static bool PromptYesNo(string message, bool defaultValue = false)
        {
            var defaultStr = defaultValue ? "Y/n" : "y/N";
            WriteColor($"{message} [{defaultStr}]: ", InfoColor);

            var input = Console.ReadLine()?.Trim().ToLower();
            
            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }

            return input == "y" || input == "yes";
        }

        public static void Pause(string message = "按任意键继续...")
        {
            WriteColor($"\n{message}", DimColor);
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}
