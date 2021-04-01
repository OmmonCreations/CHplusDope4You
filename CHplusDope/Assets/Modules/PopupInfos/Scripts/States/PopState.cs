using UnityEngine;

namespace PopupInfos
{
    public class PopTask : PopupInfoState
    {
        private const float AnimationTime = 1f;
        
        private PopupInfoLayer Layer { get; }
        
        private Vector3 _startPosition;
        private Vector2 _relativePosition;

        private float _lerp = Random.value;
        
        private float _gravity;
        private float _popForce;
        private float _distance;
        
        private Vector2 _initialMotion;
        
        private float _t;

        public PopTask(PopupInfoController controller) : base(controller)
        {
            Layer = controller.Layer;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Controller.Target.Position;
            Controller.RectTransform.anchorMin = Vector2.up;
            Controller.RectTransform.anchorMax = Vector2.up;
            _popForce = Controller.PopForce.Evaluate(0,_lerp);
            var emissionAngle = (90-Controller.EmissionAngle.Evaluate(_t,_lerp)) / 180 * Mathf.PI;
            var x = Mathf.Cos(emissionAngle);
            var y = Mathf.Sin(emissionAngle);
            _initialMotion = new Vector2(x, y) * _popForce;
            RecalculateMotion();
            UpdatePosition();
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            RecalculateMotion();
            UpdatePosition();
            Controller.Alpha = Controller.AlphaAnimation.Evaluate(_t, _lerp);
            Controller.Size = Controller.SizeAnimation.Evaluate(_t, _lerp);
            IsCompleted = _t >= 1;
        }

        private void UpdatePosition()
        {
            var position = Layer.GetViewportPosition(_startPosition);
            var rectTransform = Controller.RectTransform;
            rectTransform.anchoredPosition = position + _relativePosition;
        }

        private void RecalculateMotion()
        {
            _gravity = Controller.Gravity.Evaluate(_t, _lerp);
            _distance = Controller.Distance.Evaluate(_t, _lerp);
            _relativePosition = _initialMotion * _distance + new Vector2(0, _gravity);
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.Remove();
        }
    }
}