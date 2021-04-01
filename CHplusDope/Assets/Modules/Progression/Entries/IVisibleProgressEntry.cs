using Localizator;

namespace Progression
{
    public interface IVisibleProgressEntry : IProgressEntry, IUnlockableProgressEntry
    {
        LocalizationKey Label { get; }
        LocalizationKey HelpText { get; }
        bool IsAvailable { get; }

        void UpdateLabel();
    }
}