using System;
using UnityEngine;

namespace Views.UI
{
    public class DeviceSafeArea : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private bool _applyTop = true;
        [SerializeField] private bool _applyRight = true;
        [SerializeField] private bool _applyBottom = true;
        [SerializeField] private bool _applyLeft = true;

        private Vector2 _offsetMin;
        private Vector2 _offsetMax;
        
        public RectTransform RectTransform => _rectTransform;

        private void Awake()
        {
            _offsetMin = _rectTransform.offsetMin;
            _offsetMax = _rectTransform.offsetMax;
            DeviceSafeAreaObserver.EnsureInstancePresent();
        }

        private void OnEnable()
        {
            ApplyDeviceSafeAreas();
            DeviceSafeAreaObserver.SafeAreaChanged += ApplyDeviceSafeAreas;
        }

        private void OnDisable()
        {
            DeviceSafeAreaObserver.SafeAreaChanged -= ApplyDeviceSafeAreas;
        }

        private void Start()
        {
            ApplyDeviceSafeAreas();
        }

        public void ApplyDeviceSafeAreas()
        {
            var safeArea = Screen.safeArea;
            var canvas = GetComponentInParent<Canvas>().rootCanvas;
            if (!canvas) return;
            
            var scale = canvas.scaleFactor;
            var rectTransform = _rectTransform;

            var safeAreaMin = safeArea.min;
            var safeAreaMax = safeArea.max;
            var screenSize = new Vector2(Screen.width, Screen.height);
            var calculatedMin = safeAreaMin / scale;
            var calculatedMax = -(screenSize - safeAreaMax) / scale;
            
            var originalMin = _offsetMin;
            var originalMax = _offsetMax;
            
            var offsetMin = new Vector2(
                _applyLeft ? Mathf.Max(calculatedMin.x, originalMin.x) : originalMin.x,
                _applyBottom ? Mathf.Max(calculatedMin.y, originalMin.y) : originalMin.y
            );
            var offsetMax = new Vector2(
                _applyRight ? Mathf.Min(calculatedMax.x, originalMax.x) : originalMax.x,
                _applyTop ? Mathf.Min(calculatedMax.y, originalMax.y) : originalMax.y
            );

            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }
    }
}