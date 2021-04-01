using System.Collections.Generic;
using System.Linq;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;
using Random = RandomUtils.Random;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate disagrees and gets stuck on an obstacle (these do not reach their final position yet)
    /// </summary>
    public class DisagreeReactionsStep : CandidateReactionCalculationStep
    {
        private DisagreeReactionsStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts) : base(groupResult, course, contexts)
        {
        }

        public override void Calculate()
        {
            // Debug.Log(Contexts.Count + "/" + GroupResult.ReactionMap.Count + " disagree");
            var course = Course;
            var obstacles = course.Obstacles.OfType<IBlockingObstacle>().ToList();
            if (obstacles.Count == 0)
            {
                Debug.LogWarning("No blocking obstacles for disagreeing candidates to get stuck in!");
                return;
            }

            var entries = Contexts;
            foreach (var context in entries)
            {
                // negative time difference implies candidate needs to get further ahead, positive time difference
                // means candidate needs to fall back; skip those who need to get ahead
                if (context.TimeDifference <= 0) continue;
                
                var candidate = context.Controller;
                var from = context.Position;
                var obstacle = obstacles[Random.Range(0, obstacles.Count)];

                // try to find an opportunity for the candidate to fall back
                candidate.Speed = candidate.BaseSpeed;
                if (!obstacle.TryGetBlockOpportunity(candidate, from, context.Timestamp, out var blockOpportunity))
                {
                    // if none is found let them clear the course normally (they'll get slowed down in different ways)
                    continue;
                }

                // find a path to the blocking opportunity
                var to = blockOpportunity.Position;
                var path = CalculateReactionPath(context, from, to, context.Timestamp, candidate.Speed);
                if (path == null)
                {
                    // if there is none let them clear the course normally
                    continue;
                }

                context.Paths.Add(path);

                // make sure the path ends within the blocking opportunity time window
                var blockOpportunityStart = blockOpportunity.TimeRange.min;
                var blockOpportunityEnd = blockOpportunity.TimeRange.max;
                if (path.ClearTime < blockOpportunityStart ||
                    path.ClearTime > blockOpportunityEnd)
                {
                    path.AdjustClearTime(
                        Mathf.Clamp(path.ClearTime, blockOpportunityStart, blockOpportunityEnd),
                        action => action is MoveAction
                    );
                    context.Timestamp = path.Timestamp + path.ClearTime;
                }

                // add the block action
                var blockAction = obstacle.GetBlockAction(candidate, to, context.Timestamp, context.TimeDifference);
                path.Add(blockAction);
                context.Timestamp += blockAction.Time;

                ApplyOccupant(candidate, path);
            }
        }

        public static DisagreeReactionsStep Get(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts)
        {
            return new DisagreeReactionsStep(groupResult, course, contexts.Where(
                c => c.ReactionData.WasAlive && (c.ReactionData.Agreement < DisagreeThreshold || !c.ReactionData.IsAlive)
            ).ToList());
        }
    }
}