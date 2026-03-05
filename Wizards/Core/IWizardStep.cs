using System;

namespace RimWorldModDevProbe.Wizards.Core
{
    public interface IWizardStep
    {
        string Title { get; }
        string Description { get; }
        void Execute(WizardContext context);
        bool CanSkip { get; }
        IWizardStep NextStep { get; }
    }
}
