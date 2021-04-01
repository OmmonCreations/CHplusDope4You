using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Localizations;
using DopeElections.Races;
using DopeElections.Users;
using Essentials;
using Localizator;
using Newtonsoft.Json.Linq;
using Progression;
using UnityEngine;

namespace DopeElections.Progression
{
    public class RaceCategoryProgressEntry : ProgressEntry, IRaceProgressEntry
    {
        private int _category;

        public int Configuration
        {
            get => _category;
            set => ApplyCategory(value);
        }

        public LocalizationKey Label { get; private set; }

        public LocalizationKey UnlockedLabel => GetCategoryName();
        public LocalizationKey CompletedLabel => GetCategoryName();
        public LocalizationKey UnlockedHelpText => LKey.Views.Progression.PickCategoryEntry.HelpText;
        public LocalizationKey LockedHelpText => LKey.Views.Progression.PickCategoryEntry.HelpText;

        public bool Unlockable => true;
        public LocalizationKey HelpText => IsAvailable ? UnlockedHelpText : LockedHelpText;

        public RaceCategoryProgressEntry(NamespacedKey id,
            int category) : base(id)
        {
            _category = category;
        }

        protected override void OnLoad(JToken data)
        {
            base.OnLoad(data);
            Configuration = data != null ? (int) data : 0;
        }

        private void ApplyCategory(int category)
        {
            _category = category;
            UpdateLabel();
        }

        protected override JToken SaveData()
        {
            return Configuration > 0 ? (JToken) Configuration : null;
        }

        protected override void OnStateChanged(string state)
        {
            base.OnStateChanged(state);
            UpdateLabel();
        }

        private LocalizationKey GetCategoryName()
        {
            if (Configuration == 0) return LKey.Views.Progression.PickCategoryEntry.Label;
            var category = GetCategory();
            return category != null
                ? new LocalizationKey {fallback = category.name}
                : LKey.Views.Progression.UnknownCategoryEntry.Label;
        }

        public IRace CreateRace(RaceContext context)
        {
            var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
            var category = GetCategory();
            if (category == null) return null;

            var questions = questionnaire.Questions
                .Where(q => q.categoryId == category.id).ToArray();
            var userAnswers = questionnaire.Progression.UserAnswers;
            var firstUnanswered = questions.FirstOrDefault(q => userAnswers.All(a => a.questionId != q.id));
            var currentQuestionIndex = firstUnanswered == null ? 0 : questions.IndexOf(firstUnanswered);

            // TODO switch back to actual candidates
            var candidates = DopeElectionsApp.Instance.User.GetRegionalCandidates();
            /*
            var candidates =
                DopeElectionsApp.Instance.Assets.GetAssets<Candidate>(c => c.city == BuiltinCandidate.City);
            var agreements = new []{100, 75, 50, 25, 0};
            for (var i = 0; i < candidates.Length; i++)
            {
                var c = candidates[i];
                var agreement = agreements[MathUtil.Wrap(i, agreements.Length)];
                var responses = c.responses != null
                    ? c.responses.ToDictionary(r => r.questionId, r => r)
                    : new Dictionary<int, Response>();
                foreach (var q in questions)
                {
                    if (responses.ContainsKey(q.id)) continue;
                    responses[q.id] = new Response()
                    {
                        questionId = q.id,
                        value = agreement
                    };
                }

                c.responses = responses.Values.ToArray();
            }
            */

            var raceCandidates = candidates
                .Select(c => new RaceCandidate(c, RaceCandidateConfiguration.Default))
                .ToArray();

            // Debug.Log(raceCandidates.Length + " candidates in this race");

            if (raceCandidates.Length == 0)
            {
                Debug.LogWarning("There are no candidates in this race.\n" +
                                 DopeElectionsApp.Instance.Assets.GetAssets<Candidate>().Length +
                                 " candidates loaded.");
            }

            return new QuestionMarathon(context, raceCandidates, category, questions, currentQuestionIndex);
        }

        public QuestionCategory GetCategory()
        {
            if (Configuration == 0) return null;
            var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
            return questionnaire != null ? questionnaire.Categories.FirstOrDefault(c => c.id == Configuration) : null;
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