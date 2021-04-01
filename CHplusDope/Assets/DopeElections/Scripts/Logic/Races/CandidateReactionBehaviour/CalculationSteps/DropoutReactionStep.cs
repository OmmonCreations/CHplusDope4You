using System.Collections.Generic;
using System.Linq;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate drops out of the race by "dying" on an obstacle (these reach their final position at the obstacle)
    /// </summary>
    public class DropoutReactionsStep : CandidateReactionCalculationStep
    {
        private DropoutReactionsStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts) : base(groupResult, course, contexts)
        {
        }

        public override void Calculate()
        {
            // Debug.Log(Contexts.Count + "/" + GroupResult.ReactionMap.Count + " drop out");
            foreach (var c in Contexts)
            {
                c.Paths.Add(new RawPath(Course.NavigationMesh, new List<INavigationAction>
                {
                    new DespawnAction(c.To)
                }, c.Timestamp));
            }
        }

        public static DropoutReactionsStep Get(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts)
        {
            var dropoutContexts = contexts.Where(
                c => c.ReactionData.WasAlive && c.Candidate.IsAlive && !c.ReactionData.IsAlive
            ).ToList();
            return new DropoutReactionsStep(groupResult, course, dropoutContexts);
        }
    }
}