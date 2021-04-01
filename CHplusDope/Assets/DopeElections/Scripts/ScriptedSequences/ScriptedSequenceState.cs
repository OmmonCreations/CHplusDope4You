using Essentials;
using MobileInputs;
using StateMachines;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DopeElections.ScriptedSequences
{
    public abstract class ScriptedSequenceState : State
    {
        public event StateEvent Started = delegate { };
        public event StateEvent Skipped = delegate { };

        protected ScriptedSequenceController Controller { get; }

        public virtual SkipInputType SkipInputType { get; } = SkipInputType.None;
        public virtual SkipRange SkipRange { get; } = SkipRange.Everything;

        protected ScriptedSequenceState(ScriptedSequenceController controller)
        {
            Controller = controller;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (SkipInputType == SkipInputType.TapAnywhere)
            {
                Controller.InteractionSystem.OnPointerDown += OnPointerDown;
            }

            Started();
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            Controller.InteractionSystem.OnPointerDown -= OnPointerDown;
        }

        private void OnPointerDown(IInteractable target, InputAction.CallbackContext context)
        {
            Skip();
        }

        public virtual void Skip()
        {
            IsCompleted = true;
            OnSkip();
            Skipped();
            if (SkipRange == SkipRange.Everything)
            {
                Controller.Stop();
            }
        }

        protected virtual void OnSkip()
        {
        }
    }

    public abstract class ScriptedSequenceState<T> : ScriptedSequenceState where T : ScriptedSequenceController
    {
        protected new T Controller { get; }

        protected ScriptedSequenceState(T controller) : base(controller)
        {
            Controller = controller;
        }
    }
}