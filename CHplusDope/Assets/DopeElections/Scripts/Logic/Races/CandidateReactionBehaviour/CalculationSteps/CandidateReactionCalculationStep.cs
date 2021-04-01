using System.Collections.Generic;
using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public abstract class CandidateReactionCalculationStep
    {
        protected const int DisagreeThreshold = 45;
        protected const int AgreeThreshold = 100;
    
        protected GroupReactionResult GroupResult { get; }
        protected RaceObstacleCourse Course { get; }
        protected List<CandidateReactionContext> Contexts { get; }

        private bool PreciseOccupantTracking { get; }

        protected CandidateReactionCalculationStep(GroupReactionResult groupResult, RaceObstacleCourse course,
            List<CandidateReactionContext> contexts)
        {
            GroupResult = groupResult;
            Course = course;
            Contexts = contexts;

            PreciseOccupantTracking = course.Size.x * course.Size.y < 2000;
        }

        public abstract void Calculate();

        protected float CalculateSpeed(Vector2Int from, Vector2Int to, float expectedClearTime)
        {
            var diagonalFactor = Course.NavigationMesh.DiagonalFactor;
            var tileSize = Course.Configuration.TileSize;
            var boostLength = Mathf.Abs(to.y - from.y) + Mathf.Abs((to.x - from.x)) * (diagonalFactor - 1);
            return boostLength * tileSize / expectedClearTime;
        }

        protected RawPath CalculateReactionPath(CandidateReactionContext context, Vector2Int from,
            Vector2Int to, float timestamp, float speed)
        {
            var controller = context.Controller;
            var reaction = context.ReactionData;

            context.Controller.Speed = speed;
            var path = Course.CalculatePath(controller, from, to, timestamp, to == context.To, reaction);
            if (path == null)
            {
                Debug.LogError("Pathfinding for Candidate failed! From " + from + " to " + to);
                return null;
            }


            context.Position = to;
            context.Timestamp = path.Timestamp + path.ClearTime;

            return path;
        }

        /// <summary>
        /// Puts candidate as occupant on all tiles along the path so others can avoid occupied tiles. Executed
        /// in separate thread!
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="path"></param>
        protected void ApplyOccupant(RaceCandidateController candidate, RawPath path)
        {
            var actions = path.Actions;
            if (actions.Count == 0) return;

            var timestamp = path.Timestamp;
            var tiles = path.NavMesh.Tiles;
            for (var i = 0; i < path.Actions.Count; i++)
            {
                var action = actions[i];
                var from = action.From;
                var to = action.To;
                var toTile = tiles[to.y, to.x];
                var fromTile = from != to ? tiles[from.y, from.x] : toTile;

                if (to != from && fromTile != null)
                {
                    ApplyOccupant(candidate, from, fromTile, action, timestamp);
                }

                ApplyOccupant(candidate, to, toTile, action, timestamp);

                timestamp += action.Time;
            }
        }

        private void ApplyOccupant(RaceCandidateController candidate, Vector2Int point, ITileContent tile,
            INavigationAction action,
            float timestamp)
        {
            if (!PreciseOccupantTracking && action is MoveAction) return;
            tile.AddOccupant(new TemporaryOccupant(candidate, timestamp, timestamp + action.Time, action), point);
        }
    }
}