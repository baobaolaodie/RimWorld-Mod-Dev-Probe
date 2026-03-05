using System;
using System.Collections.Generic;

namespace RimWorldModDevProbe.Wizards.Core
{
    public class WizardResult
    {
        public bool Success { get; }
        public string Message { get; }
        public Dictionary<string, object> Data { get; }
        public Exception Error { get; }

        private WizardResult(bool success, string message, Dictionary<string, object> data, Exception error = null)
        {
            Success = success;
            Message = message ?? string.Empty;
            Data = data ?? new Dictionary<string, object>();
            Error = error;
        }

        public static WizardResult Succeeded(string message, Dictionary<string, object> data = null)
        {
            return new WizardResult(true, message, data);
        }

        public static WizardResult Failed(string message, Exception error = null)
        {
            return new WizardResult(false, message, null, error);
        }

        public static WizardResult Cancelled(string message = "Wizard cancelled by user.")
        {
            return new WizardResult(false, message, null);
        }

        public T GetData<T>(string key)
        {
            if (Data.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default;
        }

        public void PrintSummary()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"  Wizard Result: {(Success ? "SUCCESS" : "FAILED")}");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Message: {Message}");

            if (Data.Count > 0)
            {
                Console.WriteLine("\nCollected Data:");
                foreach (var kvp in Data)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }

            if (Error != null)
            {
                Console.WriteLine($"\nError: {Error.Message}");
            }
            Console.WriteLine();
        }
    }
}
