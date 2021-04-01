using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class DisappearState : GrabArmState
    {
        private const float AnimationTime = 0.5f;

        private Transform Transform { get; }

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _targetScale;

        private float _t;

        public DisappearState(GrabArmController controller) : base(controller)
        {
            Transform = controller.transform;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Transform.position;
            _startRotation = Transform.rotation;
            _startScale = Transform.localScale;
            _targetPosition = Controller.OutsideViewAnchor.position;
            _targetRotation = Controller.OutsideViewAnchor.rotation;
            _targetScale = Controller.OutsideViewAnchor.localScale;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = Mathf.SmoothStep(0, 1, _t);
            Transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);
            Transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, progress);
            Transform.localScale = Vector3.Lerp(_startScale, _targetScale, progress);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Transform.position = _targetPosition;
            Transform.rotation = _targetRotation;
            Transform.localScale = _targetScale;
            Controller.gameObject.SetActive(false);
        }
    }
}