using UnityEngine;

namespace Essentials
{
    public class AnimationCurveCheck : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _linear = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _easeInOut = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _constant = AnimationCurve.Constant(0, 1, 0.5f);

        [SerializeField] private AnimationCurve _easeIn =
            new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0.5f * Mathf.PI, 0));

        [SerializeField] private AnimationCurve _easeOut =
            new AnimationCurve(new Keyframe(0, 0, 0, Mathf.PI * 0.5f), new Keyframe(1, 1, 0, 0));
    }
}