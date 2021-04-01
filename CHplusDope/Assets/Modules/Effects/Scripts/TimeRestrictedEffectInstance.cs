using UnityEngine;

namespace Effects
{
    public abstract class TimeRestrictedEffectInstance<T> : EffectInstance<T> where T : EffectData
    {
        protected abstract float Lifetime { get; }

        private float _t = 0;
        private bool _playing = false;

        protected override void OnPlay()
        {
            base.OnPlay();
            _t = Lifetime;
            _playing = true;
        }

        protected override void Update()
        {
            if (!_playing) return;
            base.Update();
            _t -= Time.deltaTime;
            if (_t > 0) return;
            Remove();
        }

        public override void Pause()
        {
            _playing = false;
        }

        public override void Resume()
        {
            _playing = true;
        }
    }
}