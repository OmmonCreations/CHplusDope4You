using TMPro;
using UnityEngine;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(TMP_Text))]
    [ExecuteAlways]
    public class TMP_TextSizeCalculator : MonoBehaviour
    {
        [SerializeField] private float _minWidth = 0;
        [SerializeField] private float _minHeight = 0;
        [SerializeField] private float _extraWidth = 0;
        [SerializeField] private float _extraHeight = 0;

        private TMP_Text _text;
        private RectTransform _area;

        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
            _area = transform.parent as RectTransform;
        }

        public float GetRenderedWidth()
        {
            if (!_text || !gameObject.activeSelf) return 0;
            return _text.renderedWidth;
        }

        public float GetRenderedHeight()
        {
            if (!_text || !gameObject.activeSelf) return 0;
            return _text.renderedHeight;
        }

        public Vector2 GetRenderedSize()
        {
            if (!_text || !gameObject.activeSelf) return Vector2.zero;
            return _text.GetRenderedValues();
        }

        public float GetPreferredWidth()
        {
            if (!_area || !_text || !gameObject.activeSelf) return 0;
            return Mathf.Max(
                AddMargin(_text.GetPreferredValues(_text.text, float.MaxValue, _area.rect.height), _text.margin).x +
                _extraWidth, _minWidth);
        }

        public float GetPreferredHeight()
        {
            if (!_area || !_text || !gameObject.activeSelf) return 0;
            return GetPreferredHeight(_area.rect.width);
        }

        public float GetPreferredHeight(float width)
        {
            if (!_area || !_text || !gameObject.activeSelf) return 0;
            return Mathf.Max(
                AddMargin(_text.GetPreferredValues(_text.text, width, float.MaxValue), _text.margin).y + _extraHeight,
                _minHeight);
        }

        public Vector2 GetPreferredValues()
        {
            if (!_area || !_text || !gameObject.activeSelf) return Vector2.zero;
            return AddMargin(_text.GetPreferredValues(_text.text), _text.margin);
        }

        public Vector2 GetPreferredValues(string text)
        {
            if (!_area || !_text || !gameObject.activeSelf) return Vector2.zero;
            return AddMargin(_text.GetPreferredValues(text), _text.margin);
        }

        public Vector2 GetPreferredValues(string text, float width, float height)
        {
            if (!_area || !_text || !gameObject.activeSelf) return Vector2.zero;
            var size = AddMargin(_text.GetPreferredValues(text, width, height), _text.margin) +
                       new Vector2(_extraWidth, _extraHeight);
            return new Vector2(Mathf.Max(size.x, _minWidth), Mathf.Max(size.y, _minHeight));
        }

        private Vector2 AddMargin(Vector2 size, Vector4 margin)
        {
            return size + new Vector2(margin.x, margin.y) + new Vector2(margin.z, margin.w);
        }
    }
}