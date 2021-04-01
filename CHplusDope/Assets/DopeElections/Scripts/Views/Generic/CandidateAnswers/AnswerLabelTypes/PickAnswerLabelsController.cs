using DopeElections.Localizations;
using Localizator;
using UnityEngine;

namespace DopeElections.AnswerLabelTypes
{
    public class PickAnswerLabelsController : AnswerLabelsController
    {
        [SerializeField] private LocalizedText[] _labels = new LocalizedText[4];

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _labels[0].key = LKey.Components.Answer.CompletelyAgree;
            _labels[1].key = LKey.Components.Answer.SomewhatAgree;
            _labels[2].key = LKey.Components.Answer.SomewhatDisagree;
            _labels[3].key = LKey.Components.Answer.CompletelyDisagree;
        }
    }
}