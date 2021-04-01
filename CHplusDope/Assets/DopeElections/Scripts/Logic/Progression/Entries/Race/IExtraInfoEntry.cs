using Localizator;
using Progression;

namespace DopeElections.Progression
{
    public interface IExtraInfoEntry : IProgressEntry
    {
        int TalkerId { get; }
        LocalizationKey Text { get; }
        LocalizationKey LockedText { get; }
    }
}