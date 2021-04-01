using System.Linq;
using UnityEngine;

namespace DopeElections.Races
{
    public class StrollState : CandidateGroupState
    {
        private const float AccelerationSpeed = 3;

        private float Speed { get; }
        private float _currentSpeed;

        public StrollState(CandidateGroupController group) : base(group)
        {
            Speed = group.Group.CandidateConfiguration.StrollSpeed;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            foreach (var candidate in Group.Candidates.Where(c => c.IsAlive))
            {
                candidate.FollowGroup(Group);
            }
        }

        public override void Update()
        {
            if (_currentSpeed < Speed)
            {
                _currentSpeed = Mathf.Min(_currentSpeed + Time.deltaTime * AccelerationSpeed, Speed);
            }

            Group.Position += Time.deltaTime * _currentSpeed;
        }
    }
}