using StateMachines;
using UnityEngine;

namespace AnimatedObjects.Materials
{
    public class FadeableMaterialController : ToggleableMaterialController
    {
        [Tooltip("Set one value for each material")] [SerializeField]
        private string[] _shaderProperties = null;

        private float _alpha;

        public float Alpha
        {
            get => _alpha;
            set => ApplyAlpha(value);
        }

        protected override void OnShowImmediate()
        {
            base.OnShowImmediate();
            Alpha = 1;
        }

        protected override void OnHideImmediate()
        {
            base.OnHideImmediate();
            Alpha = 0;
        }

        protected override TransitionState CreateTransitionState(float a, float b, float time, AnimationCurve curve)
        {
            var state = new TransitionState(time, 0, 1);
            var from = 0;
            var to = 1;
            state.OnTransition += t =>
            {
                var progress = a + (b - a) * curve.Evaluate(t);
                Alpha = from + (to - from) * progress;
            };
            state.OnCompleted += () =>
            {
                Alpha = Mathf.Lerp(@from, to, b);
                if (b >= 1) TriggerAppeared();
                else
                {
                    gameObject.SetActive(false);
                    TriggerDisappeared();
                }
            };
            return state;
        }

        private void ApplyAlpha(float value)
        {
            _alpha = value;
            var materials = Materials;
            var properties = _shaderProperties;
            for (var i = 0; i < materials.Length; i++)
            {
                var m = materials[i];
                var property = properties[i];
                m.SetFloat(property, value);
            }
        }
    }
}