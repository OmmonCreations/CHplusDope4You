using System.Linq;
using DopeElections.Localizations;
using DopeElections.Races;
using DopeElections.Users;
using Essentials;
using Localizator;
using Progression;

namespace DopeElections.Progression
{
    public class RaceTeamEntry : ProgressEntry, IRaceProgressEntry
    {
        public LocalizationKey Label { get; private set; }

        public LocalizationKey UnlockedLabel => LKey.Views.Progression.TeamRace.Label;
        public LocalizationKey CompletedLabel => LKey.Views.Progression.TeamRace.Label;
        public LocalizationKey UnlockedHelpText => LKey.Views.Progression.TeamRace.UnlockedHelpText;
        public LocalizationKey LockedHelpText => LKey.Views.Progression.TeamRace.LockedHelpText;
        
        public LocalizationKey HelpText => IsAvailable ? UnlockedHelpText : LockedHelpText;
        
        public bool Unlockable
        {
            get
            {
                var userList = DopeElectionsApp.Instance.User.GetActiveList();
                return userList != null && userList.candidates.Any(c => c != null && c.id != default);
            }
        }

        public RaceTeamEntry(NamespacedKey id) : base(id)
        {
        }
        
        public IRace CreateRace(RaceContext context)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateLabel()
        {
            switch (State)
            {
                case ProgressState.Locked:
                    Label = LKey.Views.Progression.LockedEntry.Label;
                    break;
                case ProgressState.Unlocked:
                    Label = UnlockedLabel;
                    break;
                case ProgressState.Completed:
                    Label = CompletedLabel;
                    break;
            }
        }
    }
}