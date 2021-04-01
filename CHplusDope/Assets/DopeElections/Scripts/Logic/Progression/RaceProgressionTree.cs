using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Progression.Questionnaire;
using DopeElections.Questions;
using Essentials;
using Progression;

namespace DopeElections.Progression
{
    public class RaceProgressionTree : ProgressionTree
    {
        private const string NameSpace = DopeElectionsApp.Namespace;
        private const bool TeamRaceEnabled = false;

        private Dictionary<int, IExtraInfoEntry> _extraInfoPositions = new Dictionary<int, IExtraInfoEntry>();
        private readonly List<QuestionAnswer> _answers = new List<QuestionAnswer>();
        private readonly List<QuestionCategoryWinner> _winners = new List<QuestionCategoryWinner>();

        public IEnumerable<QuestionCategoryWinner> QuestionCategoryWinners => _winners;
        public IEnumerable<QuestionAnswer> UserAnswers => _answers;
        public IReadOnlyDictionary<int, IExtraInfoEntry> ExtraInfoPositions => _extraInfoPositions;

        public RaceProgressionTree(params IProgressEntry[] entries) :
            base(entries)
        {
        }

        private void Initialize()
        {
            PlaceExtraInfoSteps();
        }

        private void PlaceExtraInfoSteps()
        {
            _extraInfoPositions.Clear();
        
            var raceEntries = Entries.OfType<IVisibleProgressEntry>().ToList();
            var extraInfoEntries = Entries.OfType<IExtraInfoEntry>().ToList();

            var count = raceEntries.Count;
            
            const int firstExtraInfoIndex = 2;
            var lastExtraInfoIndex = raceEntries.Count - 2;
            var extraInfoStep = extraInfoEntries.Count > 1
                ? (lastExtraInfoIndex - firstExtraInfoIndex) / (float) (extraInfoEntries.Count - 1)
                : int.MaxValue;

            var nextExtraInfoPosition = (float) firstExtraInfoIndex;
            var nextExtraInfoIndex = 0;

            for (var i = 0; i < count; i++)
            {
                var hasExtraInfo = i >= nextExtraInfoPosition ||
                                   nextExtraInfoIndex < extraInfoEntries.Count && i == count - 1;
                var extraInfoEntry = hasExtraInfo ? extraInfoEntries[nextExtraInfoIndex] : null;
                if (extraInfoEntry == null) continue;

                _extraInfoPositions[i] = extraInfoEntry;
                nextExtraInfoPosition += extraInfoStep;
                nextExtraInfoIndex++;
            }
        }

        public void AddAnswer(QuestionAnswer answer)
        {
            var existing = _answers.FirstOrDefault(a => a.questionId == answer.questionId);
            if (existing != null) _answers.Remove(existing);
            _answers.Add(answer);
        }

        internal void ApplyAnswers(IEnumerable<QuestionAnswer> answers)
        {
            _answers.Clear();
            _answers.AddRange(answers);
        }

        internal void ApplyQuestionCategoryWinners(IEnumerable<QuestionCategoryWinner> winners)
        {
            _winners.Clear();
            _winners.AddRange(winners);
        }

        public void SetCategoryWinners(int categoryId, IEnumerable<QuestionCategoryWinner> winners)
        {
            foreach (var existing in _winners.Where(w => w.CategoryId == categoryId).ToList())
            {
                _winners.Remove(existing);
            }

            if (winners != null) _winners.AddRange(winners);
        }

        public float GetRelativeRaceIndex(IRaceProgressEntry entry)
        {
            var raceEntries = Entries.OfType<IRaceProgressEntry>().ToList();
            return raceEntries.IndexOf(entry) / (float) raceEntries.Count;
        }

        public static RaceProgressionTree Get(int categoryCount)
        {
            var entries = CreateEntries(categoryCount);
            var result = new RaceProgressionTree(entries);
            result.Initialize();
            return result;
        }

        private static IProgressEntry[] CreateEntries(int categoryCount)
        {
            var entries = CreateBaseEntries().ToList();
            var raceEntries = CreateRaceEntries(categoryCount, TeamRaceEnabled).ToList();

            var unlocked = true;
            for (var i = 0; i < raceEntries.Count; i++)
            {
                var entry = raceEntries[i];
                if (entry.State != ProgressEntry.ProgressState.Completed &&
                    entry is IUnlockableProgressEntry unlockable)
                {
                    entry.State = unlocked && unlockable.Unlockable
                        ? ProgressEntry.ProgressState.Unlocked
                        : ProgressEntry.ProgressState.Locked;
                    unlocked = false;
                }
            }

            foreach (var e in raceEntries.OfType<IVisibleProgressEntry>())
            {
                e.UpdateLabel();
            }

            entries.AddRange(raceEntries);

            return entries.ToArray();
        }

        private static IEnumerable<IProgressEntry> CreateBaseEntries()
        {
            return new ProgressEntry[]
            {
                new TranslationCheckEntry(RaceProgressStepId.TranslationCheck),
                new ExtraInfoEntry(
                    RaceProgressStepId.ExtraInfo1,
                    LKey.Components.Hints.MoreAccurateResults,
                    LKey.Components.Hints.TooFarWay,
                    1
                ),
                new ExtraInfoEntry(
                    RaceProgressStepId.ExtraInfo2,
                    LKey.Components.Hints.Leaderboard,
                    LKey.Components.Hints.TooFarWay,
                    2
                ),
                new ExtraInfoEntry(
                    RaceProgressStepId.ExtraInfo3,
                    LKey.Components.Hints.LikeAndDislike,
                    LKey.Components.Hints.TooFarWay,
                    3
                ),
                new ExtraInfoEntry(
                    RaceProgressStepId.ExtraInfo4,
                    LKey.Components.Hints.LeaderboardFilter,
                    LKey.Components.Hints.TooFarWay,
                    4
                ),
                new CutsceneProgressEntry(
                    RaceProgressStepId.EndingComic,
                    CutsceneId.EndingComic
                )
            };
        }

        private static IEnumerable<IProgressEntry> CreateRaceEntries(int count, bool addTeamRace = true)
        {
            var result = new ProgressEntry[count + (addTeamRace ? 1 : 0)];
            for (var i = 0; i < count; i++)
            {
                var id = i + 1;
                result[i] = new RaceCategoryProgressEntry(new NamespacedKey(NameSpace, "category_race_" + id), 0);
            }

            if (addTeamRace)
            {
                result[result.Length - 1] = new RaceTeamEntry(new NamespacedKey(NameSpace, "team_race"));
            }

            return result;
        }
    }
}