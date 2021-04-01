using UnityEngine;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class RectTransformModifier : MonoBehaviour
    {
        [SerializeField] public bool useMinWidth = false;
        [SerializeField] public bool useMaxWidth = false;
        [SerializeField] public bool useMinHeight = false;
        [SerializeField] public bool useMaxHeight = false;
        [SerializeField] public float minWidth = 0;
        [SerializeField] public float maxWidth = 0;
        [SerializeField] public float minHeight = 0;
        [SerializeField] public float maxHeight = 0;

        private RectTransform _rectTransform = null;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetMinWidth(float width)
        {
            useMinWidth = width >= 0;
            minWidth = width;
        }

        public void SetMinHeight(float height)
        {
            useMinHeight = height >= 0;
            minHeight = height;
        }

        public void SetMaxWidth(float width)
        {
            useMaxWidth = width >= 0;
            maxWidth = width;
        }

        public void SetMaxHeight(float height)
        {
            useMaxHeight = height >= 0;
            maxHeight = height;
        }

        public void SetWidth(float width)
        {
            if (!_rectTransform) return;
            var calculatedWidth = Mathf.Clamp(
                width,
                useMinWidth ? minWidth : 0,
                useMaxWidth ? maxWidth : float.MaxValue
            );
            if (_rectTransform)
                _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, calculatedWidth);
        }

        public void SetHeight(float height)
        {
            if (!_rectTransform) return;
            var calculatedHeight = Mathf.Clamp(
                height,
                useMinHeight ? minHeight : 0,
                useMaxHeight ? maxHeight : float.MaxValue
            );
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calculatedHeight);
        }

        public void SetSize(Vector2 size)
        {
            if (!_rectTransform) return;
            var calculatedSize = new Vector2(
                Mathf.Clamp(
                    size.x,
                    useMinWidth ? minWidth : 0,
                    useMaxWidth ? maxWidth : float.MaxValue
                ),
                Mathf.Clamp(
                    size.y,
                    useMinHeight ? minHeight : 0,
                    useMaxHeight ? maxHeight : float.MaxValue
                )
            );
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, calculatedSize.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calculatedSize.y);
        }
    }
}