using System.Collections.Generic;
using System.Linq;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// Candidate agrees and uses some sort of boost to get further ahead of the competition (reach their destination)
    /// </summary>
    public class AgreeReactionsStep : CandidateReactionCalculationStep
    {
        private AgreeReactionsStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts) : base(groupResult, course, contexts)
        {
        }

        public override void Calculate()
        {
            // Debug.Log(Contexts.Count + "/" + GroupResult.ReactionMap.Count + " agree");
            const float boostDistanceFraction = 0.5f;
            const float boostTimeFraction = 0.3f;

            var entries = Contexts;
            var totalClearTime = Course.AverageClearTime;
            var targetLayoutStartY = Course.Configuration.StartAreaLength + Course.Configuration.ObstacleSpaceLength;

            foreach (var context in entries)
            {
                var candidate = context.Controller;
                var from = context.From;
                var to = context.To;

                var expectedBoostClearTime = totalClearTime * boostTimeFraction;

                var middleVector = from + (to - new Vector2(from.x, from.y)) * boostDistanceFraction;
                var middle = new Vector2Int(Mathf.FloorToInt(middleVector.x), Mathf.FloorToInt(middleVector.y));

                var boostSpeed = CalculateSpeed(from, middle, expectedBoostClearTime);

                var boostPath = CalculateReactionPath(context, from, middle, context.Timestamp, boostSpeed);
                // boostPath.AdjustClearTime(expectedBoostClearTime, action => action is MoveAction);
                context.Paths.Add(boostPath);
                context.Timestamp = boostPath.Timestamp + boostPath.ClearTime;

                var calculatedBoostTime = boostPath.ClearTime;
                var adjustedChillClearTime = totalClearTime - calculatedBoostTime;
                var chillSpeed = Mathf.Max(candidate.Speed, CalculateSpeed(middle, to, adjustedChillClearTime));

                var chillPath = CalculateReactionPath(context, middle, to, context.Timestamp, chillSpeed);
                if (chillPath == null) continue;
                context.Paths.Add(chillPath);
                chillPath.AdjustClearTime(
                    adjustedChillClearTime,
                    action => action is MoveAction && action.To.y > Mathf.Min(targetLayoutStartY, to.y - 10)
                );
                context.Timestamp = chillPath.Timestamp + chillPath.ClearTime;

#if UNITY_EDITOR

                if (context.Timestamp > totalClearTime + 0.1f)
                {
                    Debug.LogWarning("Agreeing Candidate did not reach target position in time.\n" +
                              "Finish Time: " + context.Timestamp + "\n" +
                              "Expected Finish Time: " + totalClearTime + "\n" +
                              "Boost Path:\n" + boostPath + "\n" +
                              "Chill Path:\n" + chillPath);
                }

#endif

                ApplyOccupant(candidate, boostPath);
                ApplyOccupant(candidate, chillPath);
            }
        }

        public static AgreeReactionsStep Get(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts)
        {
            return new AgreeReactionsStep(groupResult, course, contexts.Where(
                c => c.ReactionData.WasAlive && c.ReactionData.IsAlive && c.ReactionData.Agreement >= AgreeThreshold
            ).ToList());
        }
    }
}