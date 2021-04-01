using Effects;
using Navigation;

namespace DopeElections.Races
{
    public class MovementParticleEffect : ParticleEffectInstance<MovementParticleEffect.EffectData>
    {
        protected override float Lifetime { get; } = float.MaxValue;
        
        private RaceCandidateController Candidate { get; set; }
        private INavigationAction EndAction { get; set; }
        private float _movementSpeed;
        
        public float MovementSpeed
        {
            get => _movementSpeed;
            set => ApplyMovementSpeed(value);
        }

        protected override void OnPlay()
        {
            base.OnPlay();
            var data = Data;
            if (data != null)
            {
                Candidate = data.Candidate;
                EndAction = data.EndAction;
                
                HookEvents(Candidate);
            }
        }

        protected override void OnRemove()
        {
            base.OnRemove();
            ReleaseHooks(Candidate);
        }

        private void HookEvents(RaceCandidateController candidate)
        {
            candidate.ActionStarted += OnActionStarted;
            candidate.ActionStopped += OnActionStopped;
            candidate.NavigationStopped += OnNavigationStopped;
        }

        private void ReleaseHooks(RaceCandidateController candidate)
        {
            candidate.ActionStarted -= OnActionStarted;
            candidate.ActionStopped -= OnActionStopped;
            candidate.NavigationStopped -= OnNavigationStopped;
        }

        private void OnActionStarted(INavigationAction action)
        {
            MovementSpeed = (action.To - action.From).magnitude / action.Time;
        }

        private void OnActionStopped(INavigationAction action)
        {
            if (action != EndAction) return;
            Remove();
        }

        private void OnNavigationStopped(INavigationAction action)
        {
            Remove();
        }

        private void ApplyMovementSpeed(float speed)
        {
            _movementSpeed = speed;
            var emission = ParticleSystem.emission;
            emission.rateOverTimeMultiplier = speed;
        }

        public new class EffectData : ParticleEffectInstance.EffectData
        {
            public RaceCandidateController Candidate { get; }
            public INavigationAction EndAction { get; }
            
            public EffectData(RaceCandidateController candidate, INavigationAction endAction) : base(float.MaxValue)
            {
                Candidate = candidate;
                EndAction = endAction;
            }
        }
    }
}