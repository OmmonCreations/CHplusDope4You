using System.Linq;
using DopeElections.Answer;
using DopeElections.Progression;
using Progression;

namespace DopeElections.Races
{
    public class ShowQuestionState : QuestionRaceState
    {
        private int _answer = -1;

        public ShowQuestionState(QuestionMarathonRaceController raceController, QuestionRace race) : base(
            raceController, race)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            RaceController.CameraController.FollowGroup();
            RaceController.Views.QuestionView.Open(Race.Question, OnAnswer);
            RaceController.OverlayController.Show();
            RaceController.OverlayController.ProgressDisplay.SetActive(Marathon.CurrentQuestionIndex);
            foreach (var subgroup in RaceController.GroupController.SubgroupControllers)
            {
                subgroup.Interactable = true;
            }
        }

        public override void Update()
        {
        }

        private void OnAnswer(int answer)
        {
            _answer = answer;
            IsCompleted = true;

            var question = Race.Question;
            var entry = Race.Marathon.Context.ProgressEntry;
            var user = DopeElectionsApp.Instance.User;
            var userAnswers = user.Questionnaire.Progression.UserAnswers.ToList();
            var questionAnswer = userAnswers.FirstOrDefault(a => a.questionId == question.id);

            if (questionAnswer == null)
            {
                questionAnswer = new QuestionAnswer(question.id, -1);
                user.Questionnaire.Progression.AddAnswer(questionAnswer);
                userAnswers.Add(questionAnswer);
            }

            questionAnswer.answer = answer;
            Race.Answer = questionAnswer;
            user.UpdateSmartSpider();

            foreach (var candidate in Marathon.Candidates)
            {
                candidate.UpdateAgreementScore(Marathon, true);
                candidate.Candidate.RecalculateMatch();
            }

            var answeredCount = Race.Marathon.Questions.Count(q => userAnswers.Any(a => a.questionId == q.id));
            var questionCount = Race.Marathon.Questions.Length;

            
            if (answeredCount >= questionCount)
            {
                Marathon.UpdateWinners();
                entry.State = ProgressEntry.ProgressState.Completed;
            }

            var firstRaceProgressEntry = user.UserJourney.GetEntry(UserJourneyStepId.FirstRace);
            if (firstRaceProgressEntry.State != ProgressEntry.ProgressState.Completed)
            {
                firstRaceProgressEntry.State = ProgressEntry.ProgressState.Completed;
            }

            user.Save();

            RaceController.OverlayController.ProgressDisplay.SetCompleted(Marathon.CurrentQuestionIndex);
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            RaceController.Views.QuestionView.Close();

            if (_answer >= 0)
            {
                RaceController.RunQuestionRace(Race);
            }
            else
            {
                RaceController.Continue();
            }
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            foreach (var subgroup in RaceController.GroupController.SubgroupControllers)
            {
                subgroup.Interactable = false;
            }
        }
    }
}