using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class LayoutTrigger : UIBehaviour
    {
        [SerializeField] private UnityEvent _onDimensionsChanged = new UnityEvent();
        [SerializeField] private UnityEvent<float> _onWidthChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent<float> _onHeightChanged = new UnityEvent<float>();
        [SerializeField] private UnityEvent<Vector2> _onSizeChanged = new UnityEvent<Vector2>();

        public UnityEvent onDimensionsChanged => _onDimensionsChanged;
        public UnityEvent<float> onWidthChanged => _onWidthChanged;
        public UnityEvent<float> onHeightChanged => _onHeightChanged;
        public UnityEvent<Vector2> onSizeChanged => _onSizeChanged;

        private RectTransform _rectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            _rectTransform = GetComponent<RectTransform>();
            TriggerUpdate();
        }

        protected override void OnDisable()
        {
            TriggerUpdate();
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            TriggerUpdate();
        }

        public void TriggerUpdate()
        {
            _onDimensionsChanged.Invoke();
            var rectTransform = _rectTransform;
            if (!rectTransform) return;
            var rect = rectTransform.rect;
            var size = rect.size;
            onWidthChanged.Invoke(size.x);
            onHeightChanged.Invoke(size.y);
            onSizeChanged.Invoke(size);
        }
    }
}