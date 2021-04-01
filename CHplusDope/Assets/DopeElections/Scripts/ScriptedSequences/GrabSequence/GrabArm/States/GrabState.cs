using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class GrabState : GrabArmState
    {
        private const float SwoopForwardTime = 0.5f;
        private const float RetractTime = 1f;

        private Transform Transform { get; }

        private Vector3 GrabOrigin { get; }
        private Vector3 GrabScale { get; }
        private Vector3 Position { get; }

        private Vector3 _startPosition;
        private Vector3 _startScale;

        private Vector3 _targetPosition;
        private Vector3 _targetScale;


        private bool _hit;

        private float _t;
        private bool _retracting;

        public GrabState(GrabArmController controller, Vector3 position, bool hit) : base(controller)
        {
            Transform = controller.transform;
            GrabOrigin = controller.GrabOriginAnchor.position;
            GrabScale = controller.GrabOriginAnchor.localScale;
            Position = position;
            _hit = hit;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (_hit) Controller.PlayGrabAnimation();
            else Controller.PlayGrabEmptyAnimation();

            _startPosition = Controller.IdleAnchor.position;
            _startScale = Controller.IdleAnchor.localScale;

            _targetPosition = Position;
            _targetScale = GrabScale;
        }

        public override void Update()
        {
            if (!_retracting) UpdateSwoopForward();
            else UpdateRectract();
        }

        private void UpdateSwoopForward()
        {
            _t += Time.deltaTime / SwoopForwardTime;
            if(_hit) _targetPosition = Controller.PlayerController.transform.position;
            var progress = Mathf.SmoothStep(0, 1, _t);
            ApplyProgress(progress);

            if (_t >= 1)
            {
                CompleteSwoopForward();
            }
        }

        private void UpdateRectract()
        {
            _t += Time.deltaTime / RetractTime;
            var progress = Mathf.SmoothStep(1, 0, _t);
            ApplyProgress(progress);
            if (_t >= 1) IsCompleted = true;
        }

        private void CompleteSwoopForward()
        {
            _t = 0;
            if (_hit)
            {
                Controller.PlayerController.transform.SetParent(Controller.PlayerAnchor, true);
                Controller.PlayerController.Grabbed();
                _startPosition = Controller.GrabbedAnchor.position;
                _startScale = Controller.GrabbedAnchor.localScale;
            }
            else Controller.CanGrab = true;
            Controller.ShowGrabFeedback(_hit);
            _retracting = true;
        }

        private void ApplyProgress(float progress)
        {
            var position = Vector3.Lerp(_startPosition, _targetPosition, progress);
            Transform.position = position;
            Transform.rotation = Quaternion.LookRotation(position - GrabOrigin, Vector3.up);
            Transform.localScale = Vector3.Lerp(_startScale, _targetScale, progress);
        }

        private void ResetTransform()
        {
            Transform.position = _startPosition;
            Transform.rotation = Quaternion.LookRotation(_startPosition - GrabOrigin, Vector3.up);
            Transform.localScale = _startScale;
        }
        
        protected override void OnComplete()
        {
            base.OnComplete();
            if(!_hit) ResetTransform();
            if (_hit)
            {
                var playerTransform = Controller.PlayerController.transform;
                var grabbedAnchor = Controller.PlayerController.GrabbedAnchor;
                playerTransform.SetParent(Controller.transform.parent, true);
                Controller.PlayerController.TransformTo(grabbedAnchor, 0.2f);
                Controller.Disappear();
            }
            else Controller.Idle();
            Controller.TriggerGrabbed(_hit);
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            if(!_hit) ResetTransform();
        }
    }
}