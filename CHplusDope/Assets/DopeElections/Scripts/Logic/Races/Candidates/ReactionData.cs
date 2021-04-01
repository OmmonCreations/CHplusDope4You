using DopeElections.Races.RaceTracks;
using UnityEngine;

namespace DopeElections.Races
{
    public class ReactionData : PositionChangeData
    {
        public Vector2Int SlotDelta { get; }

        /// <summary>
        /// Category agreement of the candidate after the reaction
        /// </summary>
        public AgreementState AgreementState { get; }

        /// <summary>
        /// Agreement of this candidate with the user on the current question, 100 for full agreement, 0 for no
        /// agreement, -1 for no answer
        /// </summary>
        public int Agreement { get; }

        /// <summary>
        /// Category agreement score before this answer
        /// </summary>
        public int PreviousAgreementScore { get; }

        public bool IsAlive { get; }
        public bool WasAlive { get; }

        public ReactionData(CandidateSlot slot, RaceTrackVector groupAnchor, Vector2Int slotDelta, int agreement,
            AgreementState agreementState, int previousAgreementScore, bool wasAlive) : base(slot, groupAnchor)
        {
            SlotDelta = slotDelta;
            Agreement = agreement;
            AgreementState = agreementState;
            PreviousAgreementScore = previousAgreementScore;
            IsAlive = slot.x >= 0 && slot.y >= 0;
            WasAlive = wasAlive;
        }
    }
}