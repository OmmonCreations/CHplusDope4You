using DopeElections.Localizations;
using Localizator;
using UnityEngine;

namespace DopeElections.AnswerLabelTypes
{
    public class SliderAnswerLabelsController : AnswerLabelsController
    {
        [SerializeField] private LocalizedText[] _labels = new LocalizedText[7];

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _labels[0].key = LKey.Components.Answer.SliderMuchMore;
            _labels[1].key = LKey.Components.Answer.SliderMore;
            _labels[2].key = LKey.Components.Answer.SliderLittleMore;
            _labels[3].key = LKey.Components.Answer.SliderSame;
            _labels[4].key = LKey.Components.Answer.SliderLittleLess;
            _labels[5].key = LKey.Components.Answer.SliderLess;
            _labels[6].key = LKey.Components.Answer.SliderMuchLess;
        }
    }
}