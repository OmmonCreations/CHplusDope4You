using DopeElections.Races.RaceTracks;
using UnityEngine;

namespace DopeElections.Races
{
    public abstract class MoveState : RaceCandidateState
    {
        protected const float DefaultSmoothing = 0.1f;

        private RaceTrack RaceTrack { get; }
        protected abstract RaceTrackVector Position { get; }
        protected float Smoothing { get; set; } = DefaultSmoothing;

        private Vector3 _velocity;

        public MoveState(RaceCandidateController candidate) : base(candidate)
        {
            RaceTrack = candidate.RaceController.RaceTrack;
        }

        public override void Update()
        {
            var position = Position;
            var targetPosition = RaceTrack.GetWorldPosition(position);
            var currentPosition = Controller.WorldPosition;
            var nextPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref _velocity, Smoothing);
            var forwardVector = nextPosition - currentPosition;
            var distance = forwardVector.magnitude;
            if (distance > 10) Debug.LogWarning(Candidate.fullName + " just moved " + distance + "m!");
            var speed = distance / Time.deltaTime;
            if (speed > 0.01f)
            {
                var currentRotation = Controller.Rotation;
                var targetRotation = Quaternion.LookRotation(forwardVector, Vector3.up);
                Controller.Rotation = Quaternion.Slerp(currentRotation, targetRotation, 0.5f);
            }

            if (float.IsNaN(nextPosition.x) || float.IsNaN(nextPosition.y) || float.IsNaN(nextPosition.z))
            {
                /*
                Debug.LogError("Trying to move to NaN vector!\n" +
                               "MoveState.Position: " + position + "\n" +
                               "Current Position: " + currentPosition + "\n" +
                               "Target Position: " + targetPosition + "\n" +
                               "Next Position: " + nextPosition + "\n" +
                               "Velocity: " + _velocity + "\n" +
                               "Smoothing: " + Smoothing);
                               */
                return;
            }

            Controller.WorldPosition = nextPosition;

            Controller.CurrentMovementSpeed = speed;
            Controller.Position = position;
        }
    }
}