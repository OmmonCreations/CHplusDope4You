using System.Collections.Generic;
using System.Linq;
using StateMachines;

namespace DopeElections.Races
{
    public class QuestionMarathonRaceController : RaceController
    {
        public QuestionMarathon Marathon { get; private set; }

        protected override int CurrentProgressIndex => Marathon != null ? Marathon.CurrentQuestionIndex : -1;

        protected override IReadOnlyList<bool> ProgressSteps
        {
            get
            {
                var userAnswers = DopeElectionsApp.Instance.User.Questionnaire.Progression.UserAnswers;
                var marathon = Marathon;
                return marathon.Questions.Select(q => userAnswers.Any(a => a.questionId == q.id)).ToList();
            }
        }

        protected override void OnBeforeRacePrepare()
        {
            base.OnBeforeRacePrepare();
            Marathon = Context.Race as QuestionMarathon;
        }

        #region States

        public override void StartRace()
        {
            StateMachine.State = new StartMarathonState(this, Marathon);
        }

        public void PrepareQuestion(QuestionRace race)
        {
            StateMachine.State = new PrepareQuestionRaceState(this, race);
        }

        public void ShowQuestion(QuestionRace race)
        {
            StateMachine.State = new ShowQuestionState(this, race);
        }

        public void RunQuestionRace(QuestionRace race)
        {
            StateMachine.State = new RunQuestionRaceState(this, race);
        }

        public void AnnounceFinishQuestion(QuestionRace race)
        {
            StateMachine.State = new AnnounceFinishQuestionRaceState(this, race);
        }

        public void Continue()
        {
            var marathon = Marathon;
            marathon.NextQuestion();
            var question = marathon.CurrentQuestion;
            var index = marathon.CurrentQuestionIndex;
            if (question != null)
            {
                var race = new QuestionRace(Marathon, question, index);
                PrepareQuestion(race);
            }
            else
            {
                GoToPostMarathonView();
            }
        }

        public void GoToPostMarathonView()
        {
            StateMachine.State = new DelayedActionState(Complete, 2f);
        }

        #endregion

        protected override void FinishRace(bool completed)
        {
            var context = Marathon != null ? Marathon.Context : null;

            // Leaving without an active marathon
            if (context == null)
            {
                DopeElectionsRouter.GoToProgress();
                return;
            }

            // Going to race result
            if (completed)
            {
                DopeElectionsRouter.GoToRaceResult(context, true);
            }

            // Going back to progress view
            else
            {
                Views.BlackMask.BlockInteractions(true);
                Views.BlackMask.FadeToBlack(() => DopeElectionsRouter.GoToProgress(context.ProgressEntry));
            }
        }

        protected override void OnProgressEntrySelected(int index)
        {
            var marathon = Marathon;
            if (marathon == null) return;
            var question = index >= 0 && index < marathon.Questions.Length ? marathon.Questions[index] : null;
            if (question == null || index == marathon.CurrentQuestionIndex) return;
            var precedingQuestions = marathon.Questions.Take(index);
            var userAnswers = DopeElectionsApp.Instance.User.Questionnaire.Progression.UserAnswers;
            var reachable = precedingQuestions.All(q => userAnswers.Any(a => a.questionId == q.id));
            if (!reachable) return;
            JumpToQuestion(index);
        }

        public void JumpToQuestion(int questionIndex)
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() =>
            {
                JumpToQuestionImmediate(questionIndex);
                Views.BlackMask.FadeToClear();
                Views.BlackMask.BlockInteractions(false);
            });
        }

        public void JumpToQuestionImmediate(int questionIndex)
        {
            var marathon = Marathon;
            var question = marathon.JumpToQuestion(questionIndex);

            SoftReset();

            StateMachine.State = new PrepareQuestionRaceState(this, new QuestionRace(marathon, question, questionIndex));
        }
    }
}