using System;
using System.Collections.Generic;
using CameraSystems;
using StateMachines;
using UnityEngine;

namespace PopupInfos
{
    public class PopupInfoLayer : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private PopupInfoController _template = null;

        public CameraSystem CameraSystem { get; private set; }

        private bool _isScreenSpaceOverlay;
        private Canvas _canvas;

        private readonly List<PopupInfoController> _controllers = new List<PopupInfoController>();

        private void OnEnable()
        {
            _canvas = _rectTransform.GetComponentInParent<Canvas>();
            _isScreenSpaceOverlay = _canvas.renderMode == RenderMode.ScreenSpaceOverlay;
        }

        public void Initialize(CameraSystem camerasystem)
        {
            CameraSystem = camerasystem;
            _template.gameObject.SetActive(false);
        }

        public PopupInfoController Show(ISpatialTargetable target, PopupInfoData data)
        {
            var resultObject = Instantiate(_template.gameObject, _rectTransform, false);
            var result = resultObject.GetComponent<PopupInfoController>();
            result.Initialize(this, target);
            result.Data = data;
            resultObject.SetActive(true);
            _controllers.Add(result);
            return result;
        }

        public Vector2 GetViewportPosition(Vector3 worldPoint)
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(CameraSystem.Camera, worldPoint);
            var relative = new Vector2(screenPoint.x / Screen.width, 1 - screenPoint.y / Screen.height);
            var rect = _rectTransform.rect;
            return new Vector2(relative.x * rect.width, -relative.y * rect.height);
        }

        internal void Remove(PopupInfoController controller)
        {
            _controllers.Remove(controller);
        }

        public void Clear()
        {
            foreach (var controller in _controllers.ToArray())
            {
                controller.Remove();
            }
        }
    }
}