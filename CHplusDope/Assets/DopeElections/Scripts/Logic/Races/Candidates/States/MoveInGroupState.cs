using System;
using DopeElections.Races.RaceTracks;
using UnityEngine;

namespace DopeElections.Races
{
    public class MoveInGroupState : MoveState
    {
        private CandidateGroup Group { get; }
        private RaceTrackVector GroupAnchor => Candidate.GroupAnchor;
        private RaceTrackVector Offset => Controller.Offset;
        private float MaxSpeed { get; }

        private RaceTrackVector _lastGroupAnchor;
        private RaceTrackVector _position;

        protected sealed override RaceTrackVector Position => _position;

        public MoveInGroupState(RaceCandidateController candidate, CandidateGroup group) : base(candidate)
        {
            Group = group;
            MaxSpeed = group.CandidateConfiguration.StrollSpeed * 3f;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _position = Controller.Position;
            _lastGroupAnchor = Candidate.GroupAnchor;
            try
            {
                Controller.PlayRunningAnimation();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public override void Update()
        {
            _position = RaceTrackVector.MoveTowards(_position, GroupAnchor + Offset, MaxSpeed * Time.deltaTime);
            base.Update();
        }
    }
}