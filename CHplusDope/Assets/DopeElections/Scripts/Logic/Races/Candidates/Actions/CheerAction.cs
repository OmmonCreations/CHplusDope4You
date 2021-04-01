
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate plays a cheering animation while running
    /// </summary>
    public class CheerAction : RaceCandidateAction, ICompilableAction
    {
        public CheerAction(Vector2Int @from, Vector2Int to, float time) : base(@from, to, time)
        {
        }
        
        public void Compile(RawPath path, int ownIndex)
        {
            
        }

        protected override void OnStarted(RaceCandidateController candidate)
        {
            base.OnStarted(candidate);
            candidate.PlayBoostAnimation();
        }
    }
}