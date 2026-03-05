using System;

namespace RimWorldModDevProbe.Wizards.Core
{
    public class WizardCancelledException : Exception
    {
        public WizardCancelledException() : base("Wizard was cancelled by user.") { }
        public WizardCancelledException(string message) : base(message) { }
    }

    public class WizardStepException : Exception
    {
        public string StepName { get; }

        public WizardStepException(string stepName, string message) : base(message)
        {
            StepName = stepName;
        }

        public WizardStepException(string stepName, string message, Exception innerException) 
            : base(message, innerException)
        {
            StepName = stepName;
        }
    }
}
