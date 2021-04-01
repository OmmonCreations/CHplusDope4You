using Essentials;
using Localizator;
using Progression;

namespace DopeElections.Progression
{
    public class ExtraInfoEntry : ProgressEntry, IExtraInfoEntry
    {
        public int TalkerId { get; }
        public LocalizationKey Text { get; }
        public LocalizationKey LockedText { get; }
    
        public ExtraInfoEntry(NamespacedKey id, LocalizationKey text, LocalizationKey lockedText, int talkerId) : base(id)
        {
            Text = text;
            LockedText = lockedText;
            TalkerId = talkerId;
        }
    }
}