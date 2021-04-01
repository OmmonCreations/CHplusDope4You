using DopeElections.Answer;
using DopeElections.Localizations;
using Localizator;
using UnityEngine;

namespace DopeElections.Races
{
    public class SliderAnswersController : QuestionAnswersController
    {
        [SerializeField] private LocalizedText _spendLessLabel = null;
        [SerializeField] private LocalizedText _spendMoreLabel = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _spendLessLabel.key = LKey.Components.Answer.SliderAxisMin;
            _spendMoreLabel.key = LKey.Components.Answer.SliderAxisMax;
        }

        protected override void OnShow(bool show)
        {
            base.OnShow(show);
            var controllers = Controllers;
            if (controllers == null) return;
            if (show)
            {
                var interval = 0.02f;
                var order = new[] {5, 2, 1, 3, 0, 4, 6};
                for (var i = 0; i < controllers.Length; i++)
                {
                    var index = order[i];
                    var c = controllers[index];
                    c.PanelController.HideImmediate();
                    c.PanelController.Show(i * interval);
                }

                UndecidedController.PanelController.HideImmediate();
                UndecidedController.PanelController.Show((controllers.Length + 1) * interval);
            }
            else
            {
                foreach(var c in controllers) c.PanelController.HideImmediate();
                UndecidedController.PanelController.HideImmediate();
            }
        }

        public override QuestionAnswer[] GetDefaultAnswers(Question question)
        {
            var questionId = question.id;
            return new[]
            {
                new QuestionAnswer(questionId, 0),
                new QuestionAnswer(questionId, 17),
                new QuestionAnswer(questionId, 34),
                new QuestionAnswer(questionId, 50),
                new QuestionAnswer(questionId, 66),
                new QuestionAnswer(questionId, 83),
                new QuestionAnswer(questionId, 100),
                new QuestionAnswer(questionId, -1),
            };
        }

        protected override LocalizationKey GetLabel(QuestionAnswer answer)
        {
            switch (answer.answer)
            {
                case 100: return LKey.Components.Answer.SliderMuchMore;
                case 83: return LKey.Components.Answer.SliderMore;
                case 66: return LKey.Components.Answer.SliderLittleMore;
                case 50: return LKey.Components.Answer.SliderSame;
                case 34: return LKey.Components.Answer.SliderLittleLess;
                case 17: return LKey.Components.Answer.SliderLess;
                case 0: return LKey.Components.Answer.SliderMuchLess;
                default: return LKey.Components.Answer.Undecided;
            }
        }
    }
}