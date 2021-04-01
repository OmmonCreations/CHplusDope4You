using System.Collections.Generic;
using System.Linq;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate has a neutral reaction and simply clears the course (reach their destinations)
    /// </summary>
    public class NeutralReactionsStep : CandidateReactionCalculationStep
    {
        private NeutralReactionsStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts) : base(groupResult, course, contexts)
        {
        }

        public override void Calculate()
        {
            // Debug.Log(Contexts.Count + "/" + GroupResult.ReactionMap.Count + " react neutral");
            var entries = Contexts;
            var totalClearTime = Course.AverageClearTime;
            var targetLayoutStartY = Course.Configuration.StartAreaLength + Course.Configuration.ObstacleSpaceLength;

            foreach (var context in entries)
            {
                var candidate = context.Controller;
                var from = context.From;
                var to = context.To;

                var speed = Mathf.Max(candidate.BaseSpeed, CalculateSpeed(from, to, totalClearTime));
                var path = CalculateReactionPath(context, from, to, context.Timestamp, speed);
                context.Paths.Add(path);
                path.AdjustClearTime(
                    totalClearTime,
                    action => action is MoveAction && action.To.y > Mathf.Min(targetLayoutStartY, to.y - 10)
                );
                context.Timestamp = path.Timestamp + path.ClearTime;

                ApplyOccupant(candidate, path);
            }
        }

        public static NeutralReactionsStep Get(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts)
        {
            return new NeutralReactionsStep(groupResult, course, contexts.Where(
                c => c.ReactionData.WasAlive && c.ReactionData.IsAlive && c.ReactionData.Agreement ==
                    Mathf.Clamp(c.ReactionData.Agreement, DisagreeThreshold, AgreeThreshold - 1)
            ).ToList());
        }
    }
}