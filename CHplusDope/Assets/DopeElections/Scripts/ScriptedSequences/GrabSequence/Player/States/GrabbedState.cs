using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class GrabbedState : GrabSequencePlayerState
    {
        private const float AnimationTime = 0.2f;
        
        private Transform Transform { get; }
        
        private Quaternion _startRotation;
        private Quaternion _targetRotation;

        private float _t;
        
        public GrabbedState(GrabSequencePlayerController playerController) : base(playerController)
        {
            Transform = playerController.transform;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startRotation = Transform.localRotation;
            _targetRotation = Quaternion.identity;
        }

        public override void Update()
        {
            _t += Time.deltaTime / AnimationTime;
            var progress = Mathf.SmoothStep(0, 1, _t);
            Transform.localRotation = Quaternion.Lerp(_startRotation, _targetRotation, progress);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Transform.localRotation = _targetRotation;
        }
    }
}