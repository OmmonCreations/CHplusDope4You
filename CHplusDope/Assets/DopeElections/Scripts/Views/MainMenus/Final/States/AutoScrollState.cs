using MobileInputs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DopeElections.MainMenus.Final
{
    public class AutoScrollState : FinalViewState
    {
        private ScrollRect ScrollRect { get; }

        private float Speed { get; }
        private float AnimationTime { get; set; }

        private float _t;
        private bool _paused = false;

        public AutoScrollState(FinalView view, float speed) : base(view)
        {
            ScrollRect = view.ScrollRect;
            Speed = speed;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var contentArea = View.ContentArea;
            var speed = Speed;
            var viewportHeight = ScrollRect.viewport.rect.height;
            AnimationTime = (contentArea.rect.height - viewportHeight) / speed;
            View.InteractionSystem.OnPointerDown += OnPointerDown;
            View.InteractionSystem.OnPointerUp += OnPointerUp;

            if (AnimationTime <= 0)
            {
                IsCompleted = true;
            }
        }

        public override void Update()
        {
            if (_paused) return;
            _t += Time.deltaTime / AnimationTime;
            ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(1 - _t);
            if (_t >= 1) IsCompleted = true;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            ScrollRect.verticalNormalizedPosition = 0;
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            View.InteractionSystem.OnPointerDown -= OnPointerDown;
            View.InteractionSystem.OnPointerUp -= OnPointerUp;
        }

        private void OnPointerDown(IInteractable target, InputAction.CallbackContext context)
        {
            _paused = true;
        }

        private void OnPointerUp(IInteractable target, InputAction.CallbackContext context)
        {
            _paused = false;
            _t = 1 - ScrollRect.verticalNormalizedPosition;
        }
    }
}