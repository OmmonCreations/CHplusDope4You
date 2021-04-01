using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;

namespace DopeElections.Races
{
    public class PickAnswersController : QuestionAnswersController
    {
        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            var controllers = Controllers;
            if (controllers == null) return;
            if (show)
            {
                var interval = 0.02f;
                for (var i = 0; i < controllers.Length; i++)
                {
                    var c = controllers[i];
                    c.PanelController.Show(i * interval);
                }

                UndecidedController.PanelController.Show((controllers.Length + 1) * interval);
            }
            else
            {
                foreach (var c in controllers) c.PanelController.HideImmediate();
                UndecidedController.PanelController.HideImmediate();
            }
        }

        public override QuestionAnswer[] GetDefaultAnswers(Question question)
        {
            var questionId = question.id;
            return new[]
            {
                new QuestionAnswer(questionId, 100),
                new QuestionAnswer(questionId, 75),
                new QuestionAnswer(questionId, 25),
                new QuestionAnswer(questionId, 0),
                new QuestionAnswer(questionId, -1),
            };
        }

        protected override LocalizationKey GetLabel(QuestionAnswer answer)
        {
            switch (answer.answer)
            {
                case 100: return LKey.Components.Answer.CompletelyAgree;
                case 75: return LKey.Components.Answer.SomewhatAgree;
                case 25: return LKey.Components.Answer.SomewhatDisagree;
                case 0: return LKey.Components.Answer.CompletelyDisagree;
                default: return LKey.Components.Answer.Undecided;
            }
        }
    }
}