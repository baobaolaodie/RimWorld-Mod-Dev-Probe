using System;
using System.Collections.Generic;
using System.Linq;

namespace RimWorldModDevProbe.Wizards.Core
{
    public abstract class WizardStepBase : IWizardStep
    {
        public virtual string Title { get; protected set; }
        public virtual string Description { get; protected set; }
        public virtual bool CanSkip => false;
        public virtual IWizardStep NextStep => null;

        protected WizardStepBase(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public abstract void Execute(WizardContext context);

        protected string ReadInput(string prompt, string defaultValue = null)
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
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }

        protected string ReadChoice(string prompt, IEnumerable<string> options, string defaultValue = null)
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

                if (string.IsNullOrEmpty(input) && defaultValue != null)
                {
                    return defaultValue;
                }

                if (int.TryParse(input, out var index) && index >= 1 && index <= optionList.Count)
                {
                    return optionList[index - 1];
                }

                if (optionList.Any(o => o.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    return optionList.First(o => o.Equals(input, StringComparison.OrdinalIgnoreCase));
                }

                ShowError($"Invalid selection. Please enter a number between 1 and {optionList.Count}.");
            }
        }

        protected int ReadInt(string prompt, int? defaultValue = null, int? minValue = null, int? maxValue = null)
        {
            while (true)
            {
                var defaultStr = defaultValue.HasValue ? defaultValue.Value.ToString() : null;
                var input = ReadInput(prompt, defaultStr);

                if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(input, out var result))
                {
                    if (minValue.HasValue && result < minValue.Value)
                    {
                        ShowError($"Value must be at least {minValue.Value}.");
                        continue;
                    }
                    if (maxValue.HasValue && result > maxValue.Value)
                    {
                        ShowError($"Value must be at most {maxValue.Value}.");
                        continue;
                    }
                    return result;
                }

                ShowError("Please enter a valid integer.");
            }
        }

        protected bool ReadBool(string prompt, bool? defaultValue = null)
        {
            var defaultStr = defaultValue.HasValue ? (defaultValue.Value ? "Y" : "N") : null;
            var hint = defaultValue.HasValue ? $" (Y/n)" : "";
            Console.Write($"{prompt}{hint}: ");

            var input = Console.ReadLine()?.Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(input) && defaultValue.HasValue)
            {
                return defaultValue.Value;
            }

            return input == "Y" || input == "YES" || input == "TRUE" || input == "1";
        }

        protected void ShowInfo(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        protected void ShowError(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ForegroundColor = originalColor;
        }

        protected void ShowSuccess(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {message}");
            Console.ForegroundColor = originalColor;
        }

        protected void ShowWarning(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] {message}");
            Console.ForegroundColor = originalColor;
        }

        protected void ShowHeader()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"  {Title}");
            Console.WriteLine(new string('=', 50));
            if (!string.IsNullOrEmpty(Description))
            {
                Console.WriteLine(Description);
                Console.WriteLine();
            }
        }

        protected void Pause(string message = "Press any key to continue...")
        {
            Console.WriteLine();
            Console.Write(message);
            Console.ReadKey(true);
            Console.WriteLine();
        }
    }
}
