using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Questions;
using Localizator;
using UnityEngine;

namespace DopeElections.Races
{
    public class QuestionMarathon : IRace
    {
        public const int MaxWinners = 3;
        public const int MaxTiedWinners = 10;
        
        private RaceCandidate[] _winners;

        public RaceContext Context { get; }
        public RaceCandidate[] Candidates { get; }

        public QuestionCategory Category { get; }
        public Question[] Questions { get; }

        public int BaseMaxScore { get; private set; }
        public int MaxScore { get; private set; }

        public int CurrentQuestionIndex { get; private set; }

        public Sprite IconWhite => Category.GetIconWhite();
        public Sprite IconOutline => Category.GetIconOutline();

        public LocalizationKey Label => new LocalizationKey {fallback = Category.name};
        public LocalizationKey MatchType => LKey.Components.Race.CategoryMatch;

        public float Progress => CurrentQuestionIndex * ProgressStepSize;
        public float ProgressStepSize => 1 / (float) Mathf.Max(1, Questions.Length - 1);

        public Question CurrentQuestion => CurrentQuestionIndex >= 0 && CurrentQuestionIndex < Questions.Length
            ? Questions[CurrentQuestionIndex]
            : null;

        public RaceCandidate[] Winners
        {
            get
            {
                if (_winners == null) _winners = GetWinners();
                return _winners;
            }
        }

        public QuestionMarathon(RaceContext context, RaceCandidate[] candidates, QuestionCategory category,
            Question[] questions, int currentQuestion)
        {
            Context = context;
            Candidates = candidates;
            Category = category;
            Questions = questions;
            CurrentQuestionIndex = currentQuestion;
        }

        public Question NextQuestion()
        {
            CurrentQuestionIndex++;
            RecalculateMaxScore();
            return CurrentQuestion;
        }

        public Question JumpToQuestion(int questionIndex)
        {
            CurrentQuestionIndex = questionIndex;

            BaseMaxScore = 0;
            RecalculateMaxScore();

            var candidates = Candidates;
            foreach (var candidate in candidates) candidate.UpdateAgreementScore(this);

            return CurrentQuestion;
        }

        public void ApplyInitialState()
        {
            JumpToQuestion(0);
        }

        public void ApplyFinishedState()
        {
            JumpToQuestion(Questions.Length);
        }

        public void UpdateWinners()
        {
            foreach (var candidate in Candidates)
            {
                candidate.Candidate.RecalculateMatch();
            }

            var sortedCandidates = Candidates
                .OrderByDescending(c => c.AgreementScore + c.match)
                .Take(MaxTiedWinners)
                .ToList();

            if (sortedCandidates.Count == 0) return;

            /*
            foreach (var c in sortedCandidates.Where(c => c.AgreementScore != 0 && !(c.match > 0)))
            {
                Debug.LogError(c.fullName + " is weird!\nAgreement: " + c.AgreementScore + "\nMatch: " + c.match + "\n" 
                               + string.Join("\n", c.Candidate.responses.Select(r => r.questionId + ": " + r.value)));
            }
            */

            var categoryId = Category.id;

            var user = DopeElectionsApp.Instance.User;
            var categoryWinners = new Dictionary<RaceCandidate, int>();
            var winners = new List<QuestionCategoryWinner>();
            var rank = 0;
            var currentScore = int.MaxValue;
            foreach (var candidate in sortedCandidates)
            {
                // agreement score values now use incremental steps of 100, match values increase in steps of 1
                var score = Mathf.RoundToInt((candidate.AgreementScore + candidate.match) * 100); 
                if (score < currentScore)
                {
                    rank++;
                    currentScore = score;
                    if (rank > MaxWinners) break;
                }

                var winner = new QuestionCategoryWinner(categoryId, rank, candidate.id);
                categoryWinners.Add(candidate, rank);
                winners.Add(winner);

                if (rank > 1 && categoryWinners.Count >= MaxWinners) break;
            }

            _winners = categoryWinners.OrderBy(e => e.Value).Select(e => e.Key).ToArray();
            user.Questionnaire.Progression.SetCategoryWinners(categoryId, winners);
        }

        private void RecalculateMaxScore()
        {
            var questionIndex = CurrentQuestionIndex;
            var userAnswers = DopeElectionsApp.Instance.User.Questionnaire.Progression.UserAnswers;
            var answeredQuestions = Questions
                .Take(questionIndex)
                .Count(q => userAnswers.Any(a => a.questionId == q.id && a.answer >= 0));
            MaxScore = BaseMaxScore + answeredQuestions * 100;
        }

        private RaceCandidate[] GetWinners()
        {
            var winnerEntries = DopeElectionsApp.Instance.User.Questionnaire.Progression.QuestionCategoryWinners
                .Where(w => w.CategoryId == Category.id)
                .OrderBy(w => w.Rank).ToList();
            var result = winnerEntries
                .Select(w => Candidates.FirstOrDefault(c => c.id == w.CandidateId))
                .ToArray();
            if (result.Any(c => c == null))
            {
                Debug.LogError("There are missing candidates!\n" + string.Join("\n",
                    winnerEntries.Select(w =>
                        w.CandidateId + ": " +
                        (Candidates.Any(c => c.id == w.CandidateId) ? "found" : "missing"))));
            }

            if (result.Length == 0)
            {
                UpdateWinners();
                DopeElectionsApp.Instance.User.Save();
                return _winners;
            }

            return result;
        }
    }
}