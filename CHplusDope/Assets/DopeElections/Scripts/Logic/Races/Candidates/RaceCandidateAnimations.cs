using UnityEngine;

namespace DopeElections.Races
{
    public class RaceCandidateAnimations : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _normalMoveCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _boostCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _jumpArcCurve = AnimationCurve.Linear(0,0,0,0);
        [SerializeField] private AnimationCurve _ballisticTimeCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _ballisticArcCurve = AnimationCurve.Linear(0,0,0,0);
        [SerializeField] private AnimationCurve _fallCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _squishWithRecoveryCurve = AnimationCurve.Linear(0,0,1,1);

        public AnimationCurve NormalMoveCurve => _normalMoveCurve;
        public AnimationCurve BoostCurve => _boostCurve;
        public AnimationCurve FallCurve => _fallCurve;
        public AnimationCurve JumpArcCurve => _jumpArcCurve;
        public AnimationCurve BallisticTimeCurve => _ballisticTimeCurve;
        public AnimationCurve BallisticArcCurve => _ballisticArcCurve;
        public AnimationCurve SquishWithRecoveryCurve => _squishWithRecoveryCurve;
    }
}