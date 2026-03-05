using System;
using System.Collections.Generic;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Wizards.Core
{
    public class DevWizard
    {
        private readonly List<IWizardStep> _steps = new List<IWizardStep>();
        private readonly ProbeContext _probeContext;
        private int _currentStepIndex;

        public List<IWizardStep> Steps => _steps;

        public DevWizard(ProbeContext probeContext)
        {
            _probeContext = probeContext ?? throw new ArgumentNullException(nameof(probeContext));
        }

        public void AddStep(IWizardStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            _steps.Add(step);
        }

        public void AddSteps(IEnumerable<IWizardStep> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));

            foreach (var step in steps)
            {
                AddStep(step);
            }
        }

        public WizardResult Run()
        {
            if (_steps.Count == 0)
            {
                return WizardResult.Failed("No steps defined in the wizard.");
            }

            var context = new WizardContext(_probeContext);
            _currentStepIndex = 0;

            PrintWelcome();

            while (_currentStepIndex < _steps.Count)
            {
                var currentStep = _steps[_currentStepIndex];

                try
                {
                    ShowStepHeader(currentStep, _currentStepIndex + 1, _steps.Count);

                    if (currentStep.CanSkip)
                    {
                        Console.Write("Skip this step? (y/N): ");
                        var skipInput = Console.ReadLine()?.Trim().ToUpperInvariant();
                        if (skipInput == "Y" || skipInput == "YES")
                        {
                            ShowInfo("Step skipped.");
                            _currentStepIndex++;
                            continue;
                        }
                    }

                    currentStep.Execute(context);
                    _currentStepIndex++;
                }
                catch (WizardCancelledException)
                {
                    return WizardResult.Cancelled();
                }
                catch (Exception ex)
                {
                    ShowError($"Step failed: {ex.Message}");
                    Console.Write("Retry? (Y/n): ");
                    var retryInput = Console.ReadLine()?.Trim().ToUpperInvariant();
                    if (retryInput != "N" && retryInput != "NO")
                    {
                        continue;
                    }
                    return WizardResult.Failed($"Step '{currentStep.Title}' failed.", ex);
                }
            }

            return WizardResult.Succeeded("Wizard completed successfully.", new Dictionary<string, object>(context.Data));
        }

        public void Reset()
        {
            _currentStepIndex = 0;
        }

        private void PrintWelcome()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("  RimWorld Mod Development Wizard");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"  Total Steps: {_steps.Count}");
            Console.WriteLine("  Type 'cancel' at any prompt to exit the wizard.");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
        }

        private void ShowStepHeader(IWizardStep step, int current, int total)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"  Step {current} of {total}: {step.Title}");
            Console.WriteLine(new string('-', 50));
            if (!string.IsNullOrEmpty(step.Description))
            {
                Console.WriteLine(step.Description);
                Console.WriteLine();
            }
        }

        private void ShowInfo(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        private void ShowError(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ForegroundColor = originalColor;
        }
    }
}
