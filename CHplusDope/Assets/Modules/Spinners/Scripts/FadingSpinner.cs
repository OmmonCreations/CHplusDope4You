using Essentials;
using UnityEngine;

namespace Spinners
{
    public class FadingSpinner : Spinner
    {
        [SerializeField] private CanvasGroup[] _steps = null;
        [SerializeField] private AnimationCurve _fadingCurve = AnimationCurve.Linear(0,0,1,1);
        
        protected override void UpdateSpinner(float t)
        {
            t = 1 - t;
            var steps = _steps;
            var count = steps.Length;
            var position = t * count;
            for (var i = 0; i < steps.Length; i++)
            {
                var distance = MathUtil.Wrap(position - i, count);
                var progress = Mathf.Clamp01(distance / (float) count);
                steps[i].alpha = _fadingCurve.Evaluate(progress);
            }
        }
    }
}
