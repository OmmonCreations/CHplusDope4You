using UnityEngine;

namespace Effects
{
    public class TrailEffectInstance : EffectInstance
    {
        [SerializeField] private TrailRenderer _trailRenderer = null;

        protected override void OnPlay()
        {
            base.OnPlay();
            _trailRenderer.Clear();
        }

        public override void Pause()
        {
            _trailRenderer.enabled = false;
        }

        public override void Resume()
        {
            _trailRenderer.enabled = true;
        }
    }
}