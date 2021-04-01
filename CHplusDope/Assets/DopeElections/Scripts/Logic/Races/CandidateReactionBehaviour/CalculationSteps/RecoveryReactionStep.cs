using System.Collections.Generic;
using System.Linq;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate got stuck and recovers (reach their destination)
    /// </summary>
    public class RecoveryReactionsStep : CandidateReactionCalculationStep
    {
        private RecoveryReactionsStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts) : base(groupResult, course, contexts)
        {
        }

        public override void Calculate()
        {
            // Debug.Log(Contexts.Count + "/" + GroupResult.ReactionMap.Count + " recover");
            var entries = Contexts;
            var courseClearTime = Course.AverageClearTime;
            var targetLayoutStartY = Course.Configuration.StartAreaLength + Course.Configuration.ObstacleSpaceLength;

            foreach (var context in entries)
            {
                if (context.Position == context.To) continue; // candidate is already at target position

                var candidate = context.Controller;
                var timestamp = context.Timestamp;
                var from = context.Position;
                var to = context.To;
                var remainingTime = courseClearTime - context.Timestamp;
                var recoverySpeed = Mathf.Max(candidate.Speed, CalculateSpeed(from, to, remainingTime));
                var path = CalculateReactionPath(context, from, to, timestamp, recoverySpeed);
                if (path == null) continue;
                context.Paths.Add(path);
                path.AdjustClearTime(
                    remainingTime,
                    action => action is MoveAction && action.To.y > Mathf.Min(targetLayoutStartY, to.y - 10)
                );
                context.Timestamp = path.Timestamp + path.ClearTime;

                ApplyOccupant(candidate, path);
            }
        }

        public static RecoveryReactionsStep Get(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts)
        {
            return new RecoveryReactionsStep(groupResult, course, contexts.Where(
                c => c.ReactionData.WasAlive && c.ReactionData.IsAlive && c.ReactionData.Agreement <= DisagreeThreshold
            ).ToList());
        }
    }
}