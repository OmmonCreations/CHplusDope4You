using TMPro;
using UnityEngine;

namespace SpeechBubbles
{
    public class TextSpeechBubbleController : SpeechBubbleController<TextSpeechBubble>
    {
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private RectTransform _textareaTransform = null;
        [SerializeField] private float _maxWidth = 400;

        private Bounds _textBounds;
        private Vector2 _preferredSize;

        protected override Vector2 PreferredSize => _preferredSize;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var textRectTransform = _text.rectTransform;
            _text.text = SpeechBubble.Text;
            textRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _maxWidth);
            _text.ForceMeshUpdate(true);
            _textBounds = _text.bounds;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;

            var textarea = _textareaTransform;
            var offsetMin = textarea.offsetMin;
            var offsetMax = textarea.offsetMax;

            var extraSize = offsetMin + -offsetMax;

            var minSize = MinSize;

            _preferredSize = new Vector2(
                Mathf.Max(minSize.x, _textBounds.size.x + extraSize.x),
                Mathf.Max(minSize.y, _textBounds.size.y + extraSize.y)
            );
        }
    }
}