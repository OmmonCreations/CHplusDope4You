using System.Collections.Generic;
using System.Linq;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate agrees and gets back into the race (reach their destination)
    /// </summary>
    public class ComebackReactionsStep : CandidateReactionCalculationStep
    {
        private ComebackReactionsStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts) : base(groupResult, course, contexts)
        {
        }

        public override void Calculate()
        {
            // Debug.Log(Contexts.Count + "/" + GroupResult.ReactionMap.Count + " candidates have a comeback");
            foreach (var c in Contexts)
            {
                c.Paths.Add(new RawPath(Course.NavigationMesh, new List<INavigationAction>
                {
                    new HideAction(c.From, c.Course.AverageClearTime),
                    new RespawnAction(c.To)
                }, 0));
            }
        }

        public static ComebackReactionsStep Get(GroupReactionResult groupResult, RaceObstacleCourse course,
            IEnumerable<CandidateReactionContext> contexts)
        {
            var comebackContexts = contexts.Where(c => !c.Candidate.IsAlive && c.ReactionData.IsAlive).ToList();
            return new ComebackReactionsStep(groupResult, course, comebackContexts);
        }
    }
}