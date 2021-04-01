using DopeElections.Races.RaceTracks;
using UnityEngine;

namespace DopeElections.Races
{
    public class RushToPartState : CandidateGroupState
    {
        private RaceTrackPartInstance Part { get; }
        private float RushTime { get; }

        private float _startPosition;
        private float _targetPosition;

        private float _t;
        
        public RushToPartState(CandidateGroupController controller, RaceTrackPartInstance part, float time = 0.5f) : base(controller)
        {
            Part = part;
            RushTime = time;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _startPosition = Group.Position;
            _targetPosition = Part.Position - Group.Length;
        }

        public override void Update()
        {
            _t += Time.deltaTime / RushTime;
            var progress = Mathf.Clamp01(_t);
            Group.Position = Mathf.Lerp(_startPosition, _targetPosition, progress);
            IsCompleted |= _t >= 1;
        }

        protected override void OnComplete()
        {
            base.OnComplete();
            Group.Position = _targetPosition;
        }
    }
}