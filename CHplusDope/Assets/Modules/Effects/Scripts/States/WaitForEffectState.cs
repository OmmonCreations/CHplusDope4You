using StateMachines;

namespace Effects
{
    public class WaitForEffectState : State
    {
        private EffectInstance Effect { get; }

        public WaitForEffectState(EffectInstance effect)
        {
            Effect = effect;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Effect.Finished += () => IsCompleted = true;
        }

        public override void Update()
        {
        }
    }
}