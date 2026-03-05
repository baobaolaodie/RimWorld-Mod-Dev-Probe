using System;
using System.Collections.Generic;
using System.Linq;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Utils;

namespace RimWorldModDevProbe.Wizards.Core
{
    public class ConsoleHelper
    {
        public static void WriteColored(string message, ConsoleColor color, string level = "INFO")
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{level}] {message}");
            Console.ResetColor();
        }

        public static string WriteLine(string message)
        {
            Console.Write($"{message} ");
            var input = Console.ReadLine()?.Trim();
            return input;
        }

        public static string ReadInput(string prompt, string defaultValue = null)
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                Console.Write($"{prompt} [{defaultValue}]: ");
            }
            else
            {
                Console.Write($"{prompt}: ");
            }

            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) return defaultValue;
            return input;
        }

        public static string ReadChoice(string prompt, IEnumerable<string> options, string defaultValue = null)
        {
            var optionList = options.ToList();
            Console.WriteLine($"\n{prompt}");
            for (int i = 0; i < optionList.Count; i++)
            {
                var marker = optionList[i] == defaultValue ? " (default)" : "";
                Console.WriteLine($"  [{i + 1}] {optionList[i]}{marker}");
            }

            while (true)
            {
                Console.Write("Select option: ");
                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    return defaultValue;
                }

                if (int.TryParse(input, out var index) && index >= 1 && index <= optionList.Count)
                {
                    return optionList[index - 1];
                }

                if (optionList.Any(o => o.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    return optionList.FirstOrDefault(o => o.Equals(input, StringComparison.OrdinalIgnoreCase));
                }

                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        public static int? ReadInt(string prompt, int? defaultValue = null, int? minValue = null, int? maxValue = null)
        {
            while (true)
            {
                var defaultStr = defaultValue.HasValue ? $" [{defaultValue.Value}]" : "";
                Console.Write($"{prompt}{defaultStr}: ");
                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input)) return defaultValue;

                if (int.TryParse(input, out var value))
                {
                    if (minValue.HasValue && value < minValue.Value)
                    {
                        Console.WriteLine($"Value must be at least {minValue.Value}");
                        continue;
                    }
                    if (maxValue.HasValue && value > maxValue.Value)
                    {
                        Console.WriteLine($"Value must be at most {maxValue.Value}");
                        continue;
                    }
                    return value;
                }

                Console.WriteLine("Invalid number. Please try again.");
            }
        }

        public static bool? ReadBool(string prompt, bool? defaultValue = null)
        {
            var hint = defaultValue.HasValue ? (defaultValue.Value ? " (Y/n)" : " (y/N)") : " (y/n)";
            Console.Write($"{prompt}{hint}: ");
            var input = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(input)) return defaultValue;

            if (input == "Y" || input == "YES") return true;
            if (input == "N" || input == "NO") return false;

            return defaultValue;
        }
    }
}
