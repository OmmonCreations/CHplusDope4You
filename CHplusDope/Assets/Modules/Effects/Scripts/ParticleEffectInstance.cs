using UnityEngine;

namespace Effects
{
    public class ParticleEffectInstance : TimeRestrictedEffectInstance<ParticleEffectInstance.EffectData>
    {
        [SerializeField] private ParticleSystem _particleSystem = null;

        private float _lifetime;

        protected override float Lifetime => _lifetime;
        protected ParticleSystem ParticleSystem => _particleSystem;

        protected override void OnPlay()
        {
            _lifetime = Data != null ? Data.Lifetime : _particleSystem.main.duration;
            base.OnPlay();
            _particleSystem.Play();
        }

        public override void Pause()
        {
            _particleSystem.Pause();
        }

        public override void Resume()
        {
            _particleSystem.Play();
        }

        protected override void OnReferenceTransformChanged(Transform t)
        {
            base.OnReferenceTransformChanged(t);
            var mainModule = _particleSystem.main;
            if (mainModule.simulationSpace != ParticleSystemSimulationSpace.World) return;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.Custom;
            mainModule.customSimulationSpace = t;
        }

        public class EffectData : Effects.EffectData
        {
            public float Lifetime { get; }

            public EffectData(float lifetime)
            {
                Lifetime = lifetime;
            }
        }
    }

    public abstract class ParticleEffectInstance<T> : ParticleEffectInstance where T : ParticleEffectInstance.EffectData
    {
        public new T Data { get; private set; }

        protected override void OnPlay()
        {
            Data = base.Data as T;
            base.OnPlay();
        }
    }
}