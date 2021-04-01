using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Navigation
{
    public class RawPath
    {
        public TileGridNavMesh NavMesh { get; }
        public float Timestamp { get; }
        public List<INavigationAction> Actions { get; }

        // public List<Vector2Int> Points { get; }
        // public List<ITileContent> Tiles { get; }
        // public List<float> Timestamps { get; }

        public float ClearTime => Actions.Sum(a => a.Time);

        public RawPath(TileGridNavMesh navMesh, List<INavigationAction> actions, float timestamp)
        {
            NavMesh = navMesh;
            Actions = actions;
            Timestamp = timestamp;
        }

        #region Public API

        public void Add(INavigationAction action)
        {
            Actions.Add(action);
            // Timestamps.Add(Timestamps.Last() + action.Time);
        }

        public void Append(RawPath path)
        {
            // Points.AddRange(path.Points.Skip(1));
            // Tiles.AddRange(path.Tiles.Skip(1));
            Actions.AddRange(path.Actions);
            // Timestamps.AddRange(path.Timestamps.Skip(1));
        }

        public void AdjustClearTime(float time)
        {
            AdjustClearTime(time, Actions);
        }

        public void AdjustClearTime(float time, Func<INavigationAction, bool> actionFilter)
        {
            var actions = Actions.Where(actionFilter).ToList();
            var excludedActionsTime = Actions.Except(actions).Sum(a => a.Time);
            var adjustedTargetTime = Mathf.Max(0, time - excludedActionsTime);
            AdjustClearTime(adjustedTargetTime, actions);
        }

        public CompiledPath Compile(TileGridNavMesh navMesh)
        {
            var navMeshTiles = navMesh.Tiles;
            var actions = Actions;
            var preCompileTiles = actions.Select(a => navMeshTiles[a.To.y, a.To.x]).ToList();
            for (var i = 0; i < preCompileTiles.Count; i++)
            {
                if (!(preCompileTiles[i] is IPathPreCompiler compiler)) continue;
                compiler.PreCompile(this, i);
            }

            for (var i = 0; i < actions.Count; i++)
            {
                if (!(actions[i] is ICompilableAction compilableAction)) continue;
                compilableAction.Compile(this, i);
            }

            var postCompileTiles = actions.Select(a => navMeshTiles[a.To.y, a.To.x]).ToList();
            for (var i = 0; i < postCompileTiles.Count; i++)
            {
                if (!(postCompileTiles[i] is IPathPostCompiler compiler)) continue;
                compiler.PostCompile(this, i);
            }

            var points = actions.Select(a => a.From).Append(actions.Last().To).ToArray();
            var tiles = points.Select(p => navMeshTiles[p.y, p.x]).ToArray();
            var timestamps = CalculateTimestamps();
            return new CompiledPath(NavMesh, points, tiles, timestamps, actions.ToArray());
        }

        #endregion

        #region Helper Methods

        private void AdjustClearTime(float time, IReadOnlyCollection<INavigationAction> actions)
        {
            var totalTime = actions.Sum(a => a.Time);
            var factor = time / totalTime;
            foreach (var a in actions) a.Time *= factor;
        }

        private float[] CalculateTimestamps()
        {
            var actions = Actions;
            var timestamp = Timestamp;
            var timestamps = new float[actions.Count + 1];
            timestamps[0] = timestamp;
            for (var i = 0; i < actions.Count; i++)
            {
                timestamp += actions[i].Time;
                timestamps[i + 1] = timestamp;
            }

            return timestamps;
        }

        #endregion

        public override string ToString() => string.Join("\n", Actions.Select((a, index) => index + ": " + a));
    }
}