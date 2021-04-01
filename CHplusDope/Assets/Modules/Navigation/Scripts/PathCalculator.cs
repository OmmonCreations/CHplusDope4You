using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Navigation
{
    public class PathCalculator
    {
        private TileGridNavMesh NavMesh { get; }
        private int MaxIterations { get; }

        public NavigationContext Context { get; private set; }
        private float DefaultClearTimePerTile { get; set; }

        public float[,] Distances { get; }
        public float[,] Timestamps { get; }
        public INavigationAction[,] Actions { get; }
        private List<PathNode> Queue { get; } = new List<PathNode>();

        private List<Vector2Int> ResultPoints { get; } = new List<Vector2Int>();

        private INavigationAgent Agent => Context.Agent;
        private Vector2Int Start => Context.From;
        private Vector2Int Target => Context.To;
        private float StartTimestamp => Context.Timestamp;

        public PathCalculator(TileGridNavMesh navMesh)
        {
            var size = new Vector2Int(navMesh.Tiles.GetLength(1), navMesh.Tiles.GetLength(0));
            NavMesh = navMesh;
            Distances = new float[size.y, size.x];
            Timestamps = new float[size.y, size.x];
            Actions = new INavigationAction[size.y, size.x];
            MaxIterations = size.x * size.y;
        }

        #region Public API

        public RawPath CalculatePath(INavigationAgent agent, INavigationQuery query)
        {
            Clear();

            Context = new NavigationContext(NavMesh, this, agent, query);

            DefaultClearTimePerTile = NavMesh.TileSize / agent.Speed;

            var start = query.From;
            var target = query.To;
            var timestamp = query.Timestamp;

            if (start == target) return null;

            var area = NavMesh.Size;

            if (Mathf.Min(start.x, target.x) < 0 ||
                Mathf.Min(start.y, target.y) < 0 ||
                Mathf.Max(start.x, target.x) >= area.x ||
                Mathf.Max(start.y, target.y) >= area.y)
            {
#if UNITY_EDITOR
                Debug.LogError("Start or Target not in navigatable area:\n" +
                               "Start: " + start + "\n" +
                               "Target: " + target + "\n" +
                               "Area: " + area);
#endif
                return null;
            }

            lock (NavMesh.Tiles)
            {
                if (!CalculateActions()) return null;
                if (!RunTraceback()) return null;
            }


            // var points = new List<Vector2Int>(ResultPoints);
            // var tiles = GetResultTiles();
            var actions = GetResultActions();
            // var timestamps = CalculateTimestamps(actions, StartTimestamp);

            if (actions.Any(a => a == null))
            {
                Debug.LogError(actions.Count(a => a == null) + " actions are missing!");
                return null;
            }

            return new RawPath(NavMesh, actions, timestamp);
        }

        public void Clear()
        {
            Context = null;

            for (var y = 0; y < Distances.GetLength(0); y++)
            {
                for (var x = 0; x < Distances.GetLength(1); x++)
                {
                    Distances[y, x] = -1;
                    Timestamps[y, x] = -1;
                    Actions[y, x] = default;
                }
            }

            Queue.Clear();
            ResultPoints.Clear();
        }

        #endregion

        #region Logic

        private bool CalculateActions()
        {
            var context = Context;
            var navMesh = NavMesh;
            var start = Start;
            var target = Target;
            var timestamp = StartTimestamp;
            var distances = Distances;
            var tiles = navMesh.Tiles;
            var actions = Actions;
            var timestamps = Timestamps;
            var queue = Queue;

            var iteration = 0;

            distances[start.y, start.x] = 0;
            timestamps[start.y, start.x] = timestamp;
            queue.Add(new PathNode(start, 0));

            var targetFound = false;

            while (iteration < MaxIterations && !targetFound && queue.Count > 0)
            {
                iteration++;
                var position = queue.First().Position;
                queue.RemoveAt(0);
                var tile = tiles[position.y, position.x];
                if (tile == null) continue;

                var currentDistance = distances[position.y, position.x];
                var currentTimestamp = timestamps[position.y, position.x];

                var availableActions = tile.GetNavigationActions(context, position, currentTimestamp);

                foreach (var action in availableActions)
                {
                    var relative = action.To;

                    // this checks whether the previous action gets to the same tile in a shorter amount of time
                    var existingTimestamp = timestamps[relative.y, relative.x];
                    if (existingTimestamp >= 0 && existingTimestamp <= currentTimestamp + action.Time) continue;

                    actions[relative.y, relative.x] = action;
                    ProcessTile(relative, currentDistance, currentTimestamp, action.Time);

                    if (relative == target)
                    {
                        // target found, stop searching
                        targetFound = true;
                        queue.Clear();
                        break;
                    }
                }
            }

            if (targetFound) return true;

            if (Context.Query.Precise)
            {
#if UNITY_EDITOR

                Debug.LogError("Target not reachable! Iterations: " + iteration + "/" + MaxIterations + "\n" +
                               "From: " + Start + ", To: " + Target + ", Area: " + NavMesh.Size);
#endif
                return false;
            }

            // default to a reachable position that is closest to the intended target
            context.To = GetClosestAlternateTarget(actions, Context.To);

            return context.To.x >= 0 && context.To.y >= 0;

            /*
            var delta = Target - Start;
            Debug.Log("Found target in " + iteration + " iterations. Taxicab distance: " +
                      Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)));
                      */
        }

        private void ProcessTile(Vector2Int position, float currentDistance, float currentTimestamp, float clearTime)
        {
            var context = Context;
            var navMesh = NavMesh;
            var tiles = navMesh.Tiles;
            var target = Target;
            var distances = Distances;
            var timestamps = Timestamps;
            var queue = Queue;

            if (position.x < 0 || position.y < 0 || position.x >= distances.GetLength(1) ||
                position.y >= distances.GetLength(0)) return;

            if (distances[position.y, position.x] >= 0 || queue.Any(n => n.Position == position)) return;

            var tile = tiles[position.y, position.x];
            if (tile != null && !tile.CanPass(context.Agent, position)) return;

            // var weight = tile != null ? tile.GetWeight(position, agent, currentTimestamp) : 1;

            var beelineDistance = (target - position).magnitude;
            var queueWeight = beelineDistance * DefaultClearTimePerTile + clearTime;

            var index = GetQueueIndex(queueWeight);
            var node = new PathNode(position, queueWeight);
            if (index >= 0) queue.Insert(index, node);
            else queue.Add(node);
            distances[position.y, position.x] = currentDistance + clearTime;
            timestamps[position.y, position.x] = currentTimestamp + clearTime;
        }

        private bool RunTraceback()
        {
            var start = Start;

            var actions = Actions;
            var resultPoints = ResultPoints;

            resultPoints.Add(Target);

            var iteration = 0;
            while (iteration < MaxIterations)
            {
                iteration++;

                var current = resultPoints.First();
                var action = actions[current.y, current.x];
                if (action == null)
                {
                    Debug.LogError("No action at " + current + "! (Traceback step: " + resultPoints.Count + ")");
                    return false;
                }

                var previous = action.From;

                resultPoints.Insert(0, previous);
                if (previous == start) break;
                if (previous == current)
                {
                    Debug.LogError("Traceback failed after " + iteration + " iterations!");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Helper Methods

        private int GetQueueIndex(float weight)
        {
            var queue = Queue;
            for (var i = 0; i < queue.Count; i++)
            {
                if (queue[i].Weight > weight) return i;
            }

            return -1;
        }

        private Vector2Int GetClosestAlternateTarget(INavigationAction[,] actions, Vector2Int intendedTarget)
        {
            var closest = new Vector2Int(-1, -1);
            var closestDistance = float.MaxValue;
            for (var y = 0; y < actions.GetLength(0); y++)
            {
                for (var x = 0; x < actions.GetLength(1); x++)
                {
                    var action = actions[y, x];
                    if (action == null) continue;

                    var actionTarget = action.To;
                    var distance = (intendedTarget - actionTarget).sqrMagnitude;
                    if (distance >= closestDistance) continue;
                    closest = actionTarget;
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private List<INavigationAction> GetResultActions()
        {
            var actions = Actions;
            var points = ResultPoints;

            return points.Skip(1).Select(p => actions[p.y, p.x]).ToList();
        }

        #endregion

        private readonly struct PathNode : IComparable
        {
            public readonly Vector2Int Position;
            public readonly float Weight;

            public PathNode(Vector2Int position, float weight)
            {
                Position = position;
                Weight = weight;
            }

            public int CompareTo(object o)
            {
                return o is PathNode n ? Weight.CompareTo(n.Weight) : 0;
            }
        }
    }
}