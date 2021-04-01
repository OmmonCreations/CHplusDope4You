using UnityEngine;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class PartyCandidateAnimations : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _jumpArcCurve = AnimationCurve.Constant(0, 1, 0);

        public AnimationCurve JumpArcCurve => _jumpArcCurve;
    }
}