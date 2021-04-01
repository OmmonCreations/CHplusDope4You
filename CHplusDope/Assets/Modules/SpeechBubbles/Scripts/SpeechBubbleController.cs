using System;
using AnimatedObjects;
using StateMachines;
using TMPro;
using UnityEngine;

namespace SpeechBubbles
{
    public abstract class SpeechBubbleController : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private RectTransform _tailTransform = null;
        [SerializeField] private Vector2 _minSize = Vector2.zero;
        [SerializeField] private float _tailLength = 40;
        [SerializeField] private ToggleableObjectController _animationController = null;

        private MonoBehaviour _target;
        private bool _isMonoTarget;
        private float _lifetime;
        private float _remainingLifetime;

        private SpeechBubbleLayer Layer { get; set; }
        public SpeechBubble SpeechBubble { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Pivot { get; private set; }

        public Vector2 MinSize
        {
            get => _minSize;
            set => _minSize = value;
        }

        public float TailLength
        {
            get => _tailLength;
            set => _tailLength = value;
        }

        internal virtual void Initialize(SpeechBubbleLayer layer, SpeechBubble speechBubble)
        {
            Layer = layer;
            SpeechBubble = speechBubble;
            _target = speechBubble.Source as MonoBehaviour;
            _lifetime = speechBubble.Time;
            _remainingLifetime = _lifetime;
            _isMonoTarget = _target;
            if (_animationController) _animationController.Show();
            OnInitialize();
            UpdateSize();

            var size = Size;
            var pivot = speechBubble.PreferredTail;
            var tip = new Vector2(size.x * pivot.x, size.y * pivot.y);
            var center = size / 2;
            var tailOrigin = tip - center;
            var tailNormal = tailOrigin.normalized;
            var startPivot = tailOrigin + tailNormal * _tailLength;
            _rectTransform.pivot = startPivot / size;
        }

        protected virtual void OnInitialize()
        {
        }

        protected void OnDestroy()
        {
            OnDestroyed();
        }

        protected virtual void OnDestroyed()
        {
        }

        protected virtual void Update()
        {
            if (_lifetime > 0 && _remainingLifetime > 0)
            {
                _remainingLifetime -= Time.deltaTime;
                if (_remainingLifetime <= 0) Remove();
            }
        }

        private void LateUpdate()
        {
            UpdatePosition();
            UpdateTail();
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void UpdateSize()
        {
            var preferredSize = PreferredSize;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
            Size = preferredSize;
        }

        protected abstract Vector2 PreferredSize { get; }

        private void UpdatePosition()
        {
            var rectTransform = _rectTransform;
            var target = SpeechBubble.Source;
            if (_isMonoTarget && !_target)
            {
                Remove();
                return;
            }

            var area = Layer.SpeechBubblesArea;
            var areaRect = area.rect;
            var areaSize = areaRect.size;
            var padding = Layer.Padding;

            var screenPoint = Layer.Camera.WorldToScreenPoint(target.Position);
            var relativeScreenPoint = new Vector2(screenPoint.x / Screen.width, 1 - screenPoint.y / Screen.height);
            var anchoredPosition = new Vector2(
                Mathf.Clamp(relativeScreenPoint.x * areaSize.x, padding.x, areaSize.x - padding.z),
                Mathf.Clamp(-relativeScreenPoint.y * areaSize.y, -areaSize.y + padding.y, -padding.y)
            );

            var size = Size;
            var sizeWithTail = size + Vector2.one * _tailLength;
            var lowerLeftOverlap = new Vector2(
                Mathf.Min(0, anchoredPosition.x - sizeWithTail.x),
                Mathf.Min(0, areaSize.y + anchoredPosition.y - sizeWithTail.y)
            );
            var upperRightOverlap = new Vector2(
                Mathf.Max(areaSize.x, anchoredPosition.x + sizeWithTail.x) - areaSize.x,
                Mathf.Max(0, anchoredPosition.y + sizeWithTail.y)
            );

            var previousPivot = rectTransform.pivot;
            var relativeTailLength = new Vector2(_tailLength / size.x, _tailLength / size.y);

            var pivotMin = new Vector2(
                -relativeTailLength.x + Mathf.Max(0, upperRightOverlap.x / size.x),
                -relativeTailLength.y + Mathf.Max(0, upperRightOverlap.y / size.y)
            );
            var pivotMax = new Vector2(
                1 + relativeTailLength.x - Mathf.Max(0, -lowerLeftOverlap.x / size.x),
                1 + relativeTailLength.y - Mathf.Max(0, -lowerLeftOverlap.y / size.y)
            );

            var pivot = new Vector2(
                Mathf.Clamp(previousPivot.x, pivotMin.x, pivotMax.x),
                Mathf.Clamp(previousPivot.y, pivotMin.y, pivotMax.y)
            );

            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            Pivot = pivot;
            Position = anchoredPosition;
        }

        private void UpdateTail()
        {
            var pivot = Pivot;
            var size = Size;
            var tip = new Vector2(size.x * pivot.x, size.y * pivot.y);
            var center = size / 2;
            var tailOrigin = tip - center;
            var tailNormal = tailOrigin.normalized;
            var tailLength = tailOrigin.magnitude;
            var angle = Vector2.SignedAngle(Vector2.down, tailNormal);
            _tailTransform.eulerAngles = new Vector3(0, 0, angle);
            _tailTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tailLength);
        }

        public TransitionState HideAfter(float time)
        {
            if (!_animationController)
            {
                Debug.LogError("There is no animated object controller attached to " + gameObject.name + " (" +
                               GetType().Name + ")");
                return null;
            }

            return _animationController.Hide(time);
        }
    }

    public abstract class SpeechBubbleController<T> : SpeechBubbleController where T : SpeechBubble
    {
        public new T SpeechBubble { get; private set; }

        internal sealed override void Initialize(SpeechBubbleLayer layer, SpeechBubble speechBubble)
        {
            SpeechBubble = speechBubble as T;
            base.Initialize(layer, speechBubble);
        }
    }
}