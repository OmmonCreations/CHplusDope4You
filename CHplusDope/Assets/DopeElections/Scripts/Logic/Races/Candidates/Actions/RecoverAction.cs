using UnityEngine;

namespace DopeElections.Races
{
    public class RecoverAction : RaceCandidateAction
    {
        public RecoverAction(Vector2Int @from, Vector2Int to, float time) : base(@from, to, time)
        {
            
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.PlayFallRecoveryAnimation();
        }
    }
}