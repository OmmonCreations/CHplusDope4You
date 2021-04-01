using System.Linq;
using DopeElections.Races.RaceTracks;
using Navigation;
using UnityEngine;
using Random = System.Random;

namespace DopeElections.Races
{
    public class ClearObstacleCourseState : MoveState
    {
        private static readonly Random Random = new Random();

        public ReactionData ReactionData { get; }
        private GroupReactionContext Context { get; }
        private CompiledPath Path { get; }
        private float GroupPosition { get; }
        private float GroupLength { get; }
        private RaceTrackVector TargetPosition { get; }

        private float _t;
        private float _actionStartTime;
        private float _actionEndTime;
        private float _actionTime;
        private RaceTrackVector _actionStartPosition;
        private RaceTrackVector _actionEndPosition;

        private INavigationAction _action;
        private int _currentActionIndex = -1;
        private RaceTrackVector _position;

#if UNITY_EDITOR
        private TileGridPathVisualizer _visualizer;
#endif

        protected override RaceTrackVector Position => _position;

        public ClearObstacleCourseState(RaceCandidateController candidate, GroupReactionContext context,
            ReactionData data, CompiledPath path) : base(candidate)
        {
            ReactionData = data;
            Context = context;
            Path = path;
            GroupLength = candidate.Group.Length;
            GroupPosition = candidate.Group.Position;
            TargetPosition = data.GroupAnchor;

            // Debug.Log(data.GroupAnchor);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _position = Controller.Position;

            Controller.ObstacleCourseController = Context.ObstacleCourseController;

            var path = Path;
            if (path == null)
            {
                IsCompleted = true;
                Controller.StateMachine.State = null;
                return;
            }

            if (ReactionData.Agreement == 100)
            {
                const float cheerTime = 0.5f;
                var cheerActionsCount = path.Timestamps.Count(t => t < cheerTime);
                for (var i = 0; i < cheerActionsCount && i < path.Actions.Length; i++)
                {
                    var originalAction = path.Actions[i];
                    if (!(originalAction is MoveAction moveAction))
                    {
                        break;
                    }

                    path.Actions[i] = new CheerAction(moveAction.From, moveAction.To, moveAction.Time);
                }
            }

            Controller.GlowEffect.Color = ReactionData.Agreement == 100 ? Color.yellow : Color.clear;
            Controller.GlowEffect.transform.localScale = Vector3.one;

            var action = path.Actions.FirstOrDefault();
            StartAction(0, action, path.Timestamp);

#if UNITY_EDITOR
            VisualizePath(path);
#endif

            Controller.SetActive(true);
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            UpdateAction();
            UpdatePosition();
            base.Update();
        }

        private void UpdateAction()
        {
            if (_t < _actionEndTime) return;

            var path = Path;
            var currentAction = _action;

            var actionIndex = GetActionIndex(_t);
            if (actionIndex < 0)
            {
                StopNavigation(currentAction);
                _action = null;
                IsCompleted = true;
                return;
            }

            for (var i = _currentActionIndex + 1; i <= actionIndex; i++)
            {
                var action = path.Actions[actionIndex];
                StartAction(actionIndex, action, _actionEndTime);
            }
        }

        private void UpdatePosition()
        {
            var relativeActionTime = _actionTime > 0 ? (_t - _actionStartTime) / _actionTime : 1;
            var position = RaceTrackVector.Lerp(_actionStartPosition, _actionEndPosition, relativeActionTime);
            if (float.IsNaN(position.x) || float.IsNaN(position.y))
            {
                Debug.LogError("Position is NaN!\n" +
                               "Action Start Position: " + _actionStartPosition + "\n" +
                               "Action End Position: " + _actionEndPosition + "\n" +
                               "Relative Action Time: " + relativeActionTime);
            }

            _position = position;
        }

        private int GetActionIndex(float timestamp)
        {
            var actions = Path.Actions;
            var t = timestamp - Path.Timestamp;
            for (var i = 0; i < actions.Length; i++)
            {
                var a = actions[i];
                if (a.Time < t)
                {
                    t -= a.Time;
                    continue;
                }

                return i;
            }

            return _currentActionIndex == actions.Length - 1 ? -1 : actions.Length - 1;
        }

        private void StartAction(int index, INavigationAction action, float timestamp)
        {
            var coursePosition = GroupPosition - GroupLength;

            var path = Path;
            var course = Context.ObstacleCourse;
            var point = action.To;
            var tileCenter = course.GetRaceTrackVector(point + new Vector2(0.5f, 0.5f), coursePosition);
            var fromTime = timestamp;
            var toTime = timestamp + action.Time;
            var previousAction = _action;

            if (index >= path.Actions.Length - 1 && ReactionData.IsAlive) tileCenter = TargetPosition;

            _action = action;
            _currentActionIndex = index;
            _actionStartTime = fromTime;
            _actionEndTime = toTime;
            _actionTime = toTime - fromTime;
            _actionStartPosition = new RaceTrackVector(_position);
            _actionEndPosition = tileCenter;

            if (previousAction != null) StopAction(previousAction);
            StartAction(action);
        }

        private void StartAction(INavigationAction action)
        {
            Controller.StartAction(action);

            if (action is RaceCandidateAction candidateAction)
            {
                Smoothing = candidateAction.MovementSmoothing;
            }
            else if (action is MoveAction _)
            {
                var previousAction = _currentActionIndex > 0 ? Path.Actions[_currentActionIndex - 1] : null;
                if (previousAction is MoveAction) return;
                Smoothing = DefaultSmoothing;
                Controller.PlayRunningAnimation();
            }
        }

        private void StopAction(INavigationAction action)
        {
            Controller.StopAction(action);
        }

        private void StopNavigation(INavigationAction lastAction)
        {
            Controller.StopNavigation(lastAction);
        }

#if UNITY_EDITOR
        private void VisualizePath(CompiledPath path)
        {
            var raceTrack = Controller.RaceController.RaceTrack;
            var course = Context.ObstacleCourse;
            var courseWidth = course.Tiles.GetLength(1) * course.Configuration.TileSize;
            var coursePosition = new RaceTrackVector(
                -courseWidth / 2, GroupPosition,
                RaceTrackVector.AxisType.Distance
            );

            var gameObject = new GameObject("Path Visualizer");
            var transform = gameObject.transform;
            transform.SetParent(RaceController.RaceTrackController.Root, false);
            transform.localPosition = raceTrack.GetWorldPosition(coursePosition);
            transform.localRotation = Quaternion.identity;

            var visualizer = gameObject.AddComponent<TileGridPathVisualizer>();
            visualizer.Initialize(Context.ObstacleCourse.NavigationMesh);
            visualizer.Path = path;
            _visualizer = visualizer;
        }
#endif

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.CurrentMovementSpeed = 0;
        }

        protected override void OnCancel()
        {
            if (_currentActionIndex < Path.Actions.Length - 1 && ReactionData.IsAlive)
            {
                var respawnAction = new RespawnAction(Path.Actions.Last().To);
                Controller.StartAction(respawnAction);
                Controller.StopAction(respawnAction);
            }

            base.OnCancel();
        }

        protected override void OnFinish()
        {
            base.OnFinish();
#if UNITY_EDITOR
            if (_visualizer && _visualizer != null) Object.Destroy(_visualizer.gameObject);
#endif
            if (_action != null)
            {
                StopAction(_action);
                StopNavigation(_action);
            }
        }
    }
}