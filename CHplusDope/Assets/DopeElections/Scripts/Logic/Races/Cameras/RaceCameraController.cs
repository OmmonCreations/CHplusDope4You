using System;
using System.Collections.Generic;
using System.Linq;
using CameraSystems;
using DopeElections.Races.RaceTracks;
using Effects;
using Essentials;
using StateMachines;
using UnityEngine;

namespace DopeElections.Races
{
    public class RaceCameraController : MonoBehaviour
    {
        public delegate void PositionEvent(float position);

        public event PositionEvent PositionChanged = delegate { };

        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private CameraSystem _cameraSystem = null;
        [SerializeField] private StateMachine _shakeExecutor = null;

        private float _currentPosition;

        private float _positionVelocity;

        // private readonly Plane[] _frustumPlanes = new Plane[6];

        public RaceController RaceController { get; private set; }

        public StateMachine StateMachine => _stateMachine;
        private CameraSystem CameraSystem => _cameraSystem;

        public Camera Camera => CameraSystem.Camera;
        // public Plane[] FrustumPlanes => _frustumPlanes;

        public float CurrentPosition
        {
            get => _currentPosition;
            set => ApplyForwardPosition(value);
        }

        public float TargetPosition { get; set; }

        private CameraTransformation BaseTransformation => RaceController.RaceTrack.PartsSet.CameraTransformation;
        public float ViewLength => RaceController.RaceTrack.PartsSet.ViewLength;

        public void Initialize(RaceController raceController)
        {
            RaceController = raceController;
            raceController.Resetted += OnRaceResetted;
        }

        private void OnEnable()
        {
            if (RaceController)
            {
                CurrentPosition = TargetPosition;
            }
        }

        private void Update()
        {
            StateMachine.Run();
            _shakeExecutor.Run();
            UpdateTransformation();
        }

        private void OnDestroy()
        {
            if (RaceController)
            {
                RaceController.Resetted -= OnRaceResetted;
            }
        }

        private void OnRaceResetted()
        {
            var group = RaceController.CandidateGroup;
            var groupLength = group.Length;

            var position = group.Position - CalculateFrontOffset(groupLength, ViewLength);

            _positionVelocity = 0;
            TargetPosition = position;
            CurrentPosition = position;

            /*
            Debug.Log("Resetted position to " + position + "\n" +
                      "View Length: " + ViewLength + "\n" +
                      "Group Length: " + groupLength);
                      */

            FollowGroup();
        }

        public void Shake(float strength, float time)
        {
            Shake(strength, time, Vector2.zero);
        }

        public void Shake(float strength, float time, Vector2 influence)
        {
            Shake shake = new PlanarShake(strength, time, influence);
            _shakeExecutor.State = new CameraShakeState(this, shake);
        }

        public CameraFollowGroupState FollowGroup()
        {
            return Follow(RaceController.CandidateControllers);
        }

        public CameraFollowGroupState Follow(IEnumerable<RaceCandidateController> candidates)
        {
            var result = new CameraFollowGroupState(this, candidates);

            StateMachine.State = result;

            return result;
        }

        public GoToPositionState Jump(float position, float time = 0.5f)
        {
            var result = GoToPositionState.EaseOut(this, position, time);
            StateMachine.State = result;
            return result;
        }

        public GoToPositionState GoTo(float position, float time = 0.5f)
        {
            var result = GoToPositionState.EaseInOut(this, position, time);
            StateMachine.State = result;
            return result;
        }

        public float CalculateFrontOffset(float groupLength, float viewLength)
        {
            var frontOffsetFromViewLength =
                viewLength / 8; // watch out: view length is usually longer than the actual view rect
            var frontOffsetFromGroupLength = groupLength / 3;
            return Mathf.Min(frontOffsetFromViewLength, frontOffsetFromGroupLength);
        }

        private void UpdateTransformation()
        {
            const float dampTime = 0.5f;

            var current = CurrentPosition;
            var target = TargetPosition;

            CurrentPosition = Mathf.SmoothDamp(current, target, ref _positionVelocity, dampTime);
        }

        private void ApplyForwardPosition(float value)
        {
            if (Math.Abs(value - _currentPosition) <= 0) return;
            _currentPosition = value;
            CameraSystem.CurrentTransform = GetTransform(value);
            PositionChanged(value);
        }

        private CameraTransformation GetTransform(float position)
        {
            return new CameraTransformation(BaseTransformation)
            {
                position = RaceController.RaceTrack.GetWorldPosition(new RaceTrackVector(0, position,
                    RaceTrackVector.AxisType.Distance))
            };
        }
    }
}