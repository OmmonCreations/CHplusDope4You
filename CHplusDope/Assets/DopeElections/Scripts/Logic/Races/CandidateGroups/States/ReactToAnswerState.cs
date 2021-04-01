using System.Collections.Generic;
using System.Linq;
using AsyncListeners;
using DopeElections.Answer;
using DopeElections.ObstacleCourses;
using DopeElections.Races.RaceTracks;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    public class ReactToAnswerState : CandidateGroupState
    {
        private Question[] Questions { get; }
        private QuestionAnswer Answer { get; }
        private IObstacleCourseGenerator CourseGenerator { get; }

        private float StrollSpeed { get; }

        private GroupReactionResult _result;
        private RaceObstacleCourse _obstacleCourse;

        private float _groupPositionBeforeCalculation;
        private float _t;
        private float _reactionTime;

        private Dictionary<RaceCandidate, CompiledPath> _pathsMap;
        private bool _pathsCalculated = false;
        private bool _cinematicStarted = false;

        private ClearObstacleCourseState[] _candidateStates;

        private RaceObstacleCourseController _courseController;

        public ReactToAnswerState(CandidateGroupController group, IObstacleCourseGenerator generator,
            Question[] questions, QuestionAnswer answer) : base(group)
        {
            Questions = questions;
            Answer = answer;
            CourseGenerator = generator;
            StrollSpeed = group.Group.CandidateConfiguration.StrollSpeed;
        }

        #region Control

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var result = GroupReactionResult.Calculate(Group, Questions, Answer, CourseGenerator);
            var obstacleCourse = GenerateObstacleCourse(result);

            _groupPositionBeforeCalculation = Group.Position;

            CandidateReactionsResult.Calculate(result, obstacleCourse, RaceController.CandidateControllers).Then(
                pathsMap =>
                {
                    _pathsMap = pathsMap;
                    _pathsCalculated = true;
                });

            _result = result;
            _obstacleCourse = obstacleCourse;
        }

        public override void Update()
        {
            if (!_pathsCalculated)
            {
                Group.Position += Time.deltaTime * StrollSpeed;
                return;
            }

            if (!_cinematicStarted)
            {
                _cinematicStarted = true;
                StartCinematic();
            }

            _t += Time.deltaTime;
            IsCompleted |= _t >= _reactionTime;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            // UpdateCameraFocus(_result);
            ApplyResultState();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            ApplyResultState();
            foreach (var c in Group.Candidates) c.ResetState();
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            if (_courseController)
            {
                _courseController.StopObstacles();
            }
        }

        #endregion

        #region Logic

        private void StartCinematic()
        {
            var result = _result;
            var obstacleCourse = _obstacleCourse;

            // since candidates continue moving during calculation we have to adjust for it before triggering reactions
            var calculationDistance = Group.Position - _groupPositionBeforeCalculation;
            var adjustedTargetPosition = result.TargetPosition + calculationDistance;

            result.AdjustTargetPosition(adjustedTargetPosition);

            // also make sure the track is long enough for the new positions
            RaceController.RaceTrack.MainGenerator.CreateParts(adjustedTargetPosition);

            // calculate how long it takes for all relevant reactions to finish
            var reactionTime = _pathsMap
                .Where(e => result.ReactionMap.TryGetValue(e.Key, out var r) && r.IsAlive)
                .Select(e => e.Value.Timestamp + e.Value.Time)
                .DefaultIfEmpty()
                .Max();
            _reactionTime = reactionTime;

            // create obstacle course view
            var courseController = CreateObstacleCourseController(obstacleCourse);
            _courseController = courseController;

            // trigger candidate reactions
            TriggerReactions(result, courseController);

            // hide candidate subgroups during reaction animation
            GroupController.HideSubgroups();

            RaceController.CameraController.Follow(RaceController.CandidateControllers);
            // Debug.Log("Group Reaction: " + _reactionTime + "s vs " + _obstacleCourse.AverageClearTime + "s");
        }

        private void TriggerReactions(GroupReactionResult result, RaceObstacleCourseController obstacleCourseController)
        {
            var group = Group;
            var layout = result.TargetLayout;
            var maxAgreementScore = result.MaxAgreementScore;
            var reactionMap = result.ReactionMap;
            var obstacleCourse = obstacleCourseController.Course;
            var reactionTime = obstacleCourse.AverageClearTime;
            var context = new GroupReactionContext(layout, obstacleCourseController, maxAgreementScore, reactionTime);

            var paths = _pathsMap;
            foreach (var candidate in group.Candidates)
            {
                var reaction = reactionMap.TryGetValue(candidate, out var r) ? r : null;
                var path = paths.TryGetValue(candidate, out var p) ? p : null;
                candidate.ReactToAnswer(context, reaction, path);
            }

            _candidateStates = RaceController.CandidateControllers
                .Select(c => c.StateMachine.State as ClearObstacleCourseState)
                .Where(s => s != null && s.ReactionData.IsAlive)
                .ToArray();
        }

        private void ApplyResultState()
        {
            var result = _result;
            Group.SetGroups(result.Groups);
            Group.SetPositionWithLayout(result.TargetPosition, result.TargetLayout);
        }

        private RaceObstacleCourse GenerateObstacleCourse(GroupReactionResult reactionResult)
        {
            var group = Group;
            var courseWidth = reactionResult.ObstacleCourseWidth;
            var courseLength = reactionResult.ObstacleCourseLength;
            var currentGroupPosition = group.Position;
            var currentGroupLength = group.Length;
            var targetGroupLength = reactionResult.TargetLayout.Length;

            /*
            Debug.Log("Course length: " + courseLength + ", Current group length: " + currentGroupLength +
                      ", Target group length: " + targetGroupLength);
                      */

            var obstacleSpaceLength = courseLength - (currentGroupLength + targetGroupLength);

            var candidateControllers = RaceController.CandidateControllers;
            var aliveCount = reactionResult.ReactionMap.Count(e => e.Value.IsAlive);
            var averageAgreeingSlotDelta = reactionResult.ReactionMap.Where(e => e.Value.Agreement == 100)
                .Select(e => e.Value.SlotDelta.y)
                .DefaultIfEmpty()
                .Average();
            var jokerUsers = reactionResult.ReactionMap
                // slot y0 is the front line, so a more negative y value means getting further ahead
                .Where(e =>
                    // must agree fully
                    e.Value.Agreement == 100 &&
                    // must be alive before and after reaction
                    e.Value.IsAlive && e.Value.WasAlive &&
                    // must be better than the average of agreeing candidates
                    e.Value.SlotDelta.y < averageAgreeingSlotDelta
                )
                // push the ones with the greatest slot delta the beginning of the collection
                .OrderBy(e => e.Value.SlotDelta.y)
                // take only the best ones
                .Take(Mathf.Min(Mathf.FloorToInt(aliveCount / 10f), 5))
                // select their controllers
                .Select(e => candidateControllers.FirstOrDefault(c => c.Candidate == e.Key))
                // only take the ones that have a controller
                .Where(c => c != null);
            var referenceStartPoint = currentGroupPosition - currentGroupLength;

            var generator = CourseGenerator;
            var tileSize = generator.TileSize; // / 3;

            var factory = new RaceObstacleCourseFactory();
            return factory
                .SetCandidateConfiguration(group.CandidateConfiguration)
                .SetStartAreaLength(currentGroupLength)
                .SetObstacleSpaceLength(obstacleSpaceLength)
                .SetWidth(courseWidth)
                .SetLength(courseLength)
                .SetTileSize(tileSize)
                .SetJokerUsers(jokerUsers, referenceStartPoint)
                .SetGenerator(generator)
                .Build();
        }

        private RaceObstacleCourseController CreateObstacleCourseController(RaceObstacleCourse course)
        {
            var raceController = RaceController;
            var raceTrack = raceController.RaceTrack;
            var courseWidth = course.Tiles.GetLength(1) * course.Configuration.TileSize;
            var coursePosition = new RaceTrackVector(
                -courseWidth / 2, Group.Position - Group.Length,
                RaceTrackVector.AxisType.Distance
            );

            var gameObject = new GameObject("Obstacle Course");
            var transform = gameObject.transform;
            transform.SetParent(raceController.RaceTrackController.Root, false);
            transform.localPosition = raceTrack.GetWorldPosition(coursePosition);
            transform.localRotation = Quaternion.identity;

            var result = gameObject.AddComponent<RaceObstacleCourseController>();
            result.Initialize(raceController, course, coursePosition);
            result.PlayAppearAnimation();
            return result;
        }

        #endregion
    }
}