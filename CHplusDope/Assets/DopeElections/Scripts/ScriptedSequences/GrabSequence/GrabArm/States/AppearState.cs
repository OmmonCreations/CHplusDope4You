using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class AppearState : GrabArmState
    {
        private const float AnimationTime = 3f;

        private Transform Transform { get; }

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _targetScale;

        private float _t;

        public AppearState(GrabArmController controller) : base(controller)
        {
            Transform = controller.transform;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Controller.OutsideViewAnchor.position;
            _startRotation = Controller.OutsideViewAnchor.rotation;
            _startScale = Controller.OutsideViewAnchor.localScale;
            _targetPosition = Controller.IdleAnchor.position;
            _targetRotation = Quaternion.LookRotation(
                _targetPosition - Controller.GrabOriginAnchor.position,
                Vector3.up
            );
            _targetScale = Controller.IdleAnchor.localScale;

            Transform.position = _startPosition;
            Transform.rotation = _startRotation;
            Transform.localScale = _startScale;

            Controller.gameObject.SetActive(true);
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = 1 - Mathf.Pow(1 - _t, 3);
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
            Controller.Idle();
        }
    }
}