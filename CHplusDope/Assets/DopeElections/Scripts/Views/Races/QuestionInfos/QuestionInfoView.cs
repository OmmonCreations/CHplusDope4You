using System.Collections;
using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.Localizations;
using Essentials;
using Html;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Races.QuestionInfos
{
    public class QuestionInfoView : RaceView, IView<Question>
    {
        public override NamespacedKey Id => RaceViewId.QuestionInfo;

        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _closeBackground = null;
        [SerializeField] private HtmlCanvas _htmlCanvas = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ToggleableObjectController _animationController = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _closeButton.onClick.AddListener(Close);
            _closeBackground.onClick.AddListener(Close);
            _confirmText.key = LKey.Views.QuestionInfo.Confirm;
            _animationController.HideImmediate();
        }

        public void Open(Question data)
        {
            _htmlCanvas.Html = data.info;
            base.Open();
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _animationController.Show();
            StartCoroutine(SetScrollPosition()); // yikes but oh well
            _scrollRect.verticalNormalizedPosition = 1;
        }

        protected override StateChangePromise PrepareClose()
        {
            var result = new StateChangePromise();
            _animationController.Hide().Then(result.Fulfill);
            return result;
        }

        // Yes I know this is a Coroutine and it is super ugly but in this special case more convenient than doing
        // the same in another way. The error potential is small in this particular case since it only runs for
        // one frame.
        private IEnumerator SetScrollPosition()
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 1;
        }
    }
}