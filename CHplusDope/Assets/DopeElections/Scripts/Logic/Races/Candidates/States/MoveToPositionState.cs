using DopeElections.Races.RaceTracks;
using UnityEngine;

namespace DopeElections.Races
{
    public class MoveToPositionState : MoveState
    {
        private RaceTrackVector TargetPosition { get; }
        private float TotalTime { get; }
        private AnimationCurve AnimationCurve { get; }
        
        private float _t;
        private RaceTrackVector _startPosition;
        private RaceTrackVector _position;

        protected override RaceTrackVector Position => _position;

        public MoveToPositionState(RaceCandidateController candidate, RaceTrackVector targetPosition, float time)
            : base(candidate)
        {
            TargetPosition = targetPosition;
            TotalTime = time;
            AnimationCurve = candidate.Animations.NormalMoveCurve;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Controller.Position;
            Controller.PlayRunningAnimation();
        }

        public override void Update()
        {
            _t += Time.deltaTime / TotalTime;
            var progress = AnimationCurve.Evaluate(Mathf.Clamp01(_t));
            _position = RaceTrackVector.Lerp(_startPosition, TargetPosition, progress);
            IsCompleted |= _t >= 1;
            base.Update();
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Controller.Position = TargetPosition;
        }
    }
}