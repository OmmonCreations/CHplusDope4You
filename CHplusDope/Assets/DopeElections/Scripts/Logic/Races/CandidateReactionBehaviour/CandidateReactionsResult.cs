using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using AsyncListeners;
using DopeElections.ObstacleCourses;
using Navigation;

namespace DopeElections.Races
{
    public class CandidateReactionsResult
    {
        private List<CandidateReactionContext> Contexts { get; }
        private CandidateReactionCalculationStep[] Steps { get; }

        private bool _done;
        private Action<Dictionary<RaceCandidate, CompiledPath>> _resolve;

        private readonly Stopwatch _stopwatch;

        private CandidateReactionsResult(List<CandidateReactionContext> contexts,
            params CandidateReactionCalculationStep[] steps)
        {
            Contexts = contexts;
            Steps = steps;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _stopwatch = stopwatch;
        }

        #region Public API

        public void Then(Action<Dictionary<RaceCandidate, CompiledPath>> resolve)
        {
            lock (Contexts)
            {
                _resolve = resolve;
            }

            if (_done)
            {
                lock (Contexts)
                {
                    Resolve(Contexts);
                }

                return;
            }


            AsyncOperations.Await(() => _done).OnComplete += () =>
            {
                lock (Contexts) Resolve(Contexts);
            };
        }

        #endregion

        #region Logic

        private void CalculateReactions()
        {
            foreach (var s in Steps) s.Calculate();
            _done = true;
        }

        private void Resolve(List<CandidateReactionContext> reactionsMap)
        {
            _stopwatch.Stop();
            // var calculatedPathsCount = Contexts.Sum(c => c.Paths.Count);
            // var candidatesCount = Contexts.Count;
            //Debug.Log("Reactions calculated in " + _stopwatch.ElapsedMilliseconds + "ms");

            if (_resolve != null)
            {
                var pathsMap = new Dictionary<RaceCandidate, CompiledPath>();
                foreach (var e in reactionsMap)
                {
                    var path = e.Paths.FirstOrDefault();
                    if (path == null) continue;
                    foreach (var p in e.Paths.Skip(1)) path.Append(p);
                    pathsMap[e.Candidate] = path.Compile(e.Course.NavigationMesh);
                }

                // Debug.Log(pathsMap.Count + " of " + candidatesCount + " full paths calculated.\n" +
                //           reactionsMap.Count(e => e.Position != e.To) + " candidates did not reach their destination.");

                _resolve(pathsMap);
            }
        }

        #endregion

        #region Static Helpers

        private static List<CandidateReactionContext> CreateReactionContexts(GroupReactionResult groupResult,
            RaceObstacleCourse course, IEnumerable<RaceCandidateController> candidates)
        {
            var reactionMap = groupResult.ReactionMap;
            return candidates
                .ToDictionary(c => c, c => reactionMap.TryGetValue(c.Candidate, out var r) ? r : null)
                .Where(e => e.Key.Candidate.IsAlive || e.Value.IsAlive)
                .Select(c => CreateReactionContext(groupResult, course, c.Key))
                .ToList();
        }

        private static CandidateReactionContext CreateReactionContext(GroupReactionResult groupResult,
            RaceObstacleCourse course, RaceCandidateController controller)
        {
            var candidate = controller.Candidate;
            var group = groupResult.Group;
            var reactionsMap = groupResult.ReactionMap;
            var reaction = reactionsMap.TryGetValue(candidate, out var r) ? r : null;
            if (reaction == null) return null;

            var anchorMap = groupResult.GroupAnchorMap;
            var coursePosition = group.Position - group.Length;

            var start = candidate.GroupAnchor;
            var target = anchorMap.TryGetValue(candidate, out var s) ? s : default;
            var startTile = course.GetClosestEmptyTile(controller, start, coursePosition);
            var endTile = course.GetClosestEmptyTile(controller, target, coursePosition);

            var waitTime = reaction.IsAlive
                ? reaction.SlotDelta.y * group.LayoutConfiguration.SlotSize / candidate.BaseSpeed
                : float.MaxValue;
            var result = new CandidateReactionContext(course, controller, reaction, startTile, endTile, waitTime);
            controller.Candidate.LastReactionContext = result;
            #if UNITY_EDITOR
            controller.UpdateDebugInfo();
            #endif
            return result;
        }

        #endregion

        public static CandidateReactionsResult Calculate(GroupReactionResult groupResult, RaceObstacleCourse course,
            IEnumerable<RaceCandidateController> candidates)
        {
            var contexts = CreateReactionContexts(groupResult, course, candidates);
            var result = new CandidateReactionsResult(contexts,
                DisagreeReactionsStep.Get(groupResult, course, contexts),
                DropoutReactionsStep.Get(groupResult, course, contexts),
                NeutralReactionsStep.Get(groupResult, course, contexts),
                AgreeReactionsStep.Get(groupResult, course, contexts),
                ComebackReactionsStep.Get(groupResult, course, contexts),
                RecoveryReactionsStep.Get(groupResult, course, contexts)
            );
            var thread = new Thread(result.CalculateReactions);
            thread.Start();
            return result;
        }
    }
}