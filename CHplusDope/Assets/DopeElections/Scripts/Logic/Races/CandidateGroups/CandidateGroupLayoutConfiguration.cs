using UnityEngine;

namespace DopeElections.Races
{
    [System.Serializable]
    public class CandidateGroupLayoutConfiguration
    {
        [SerializeField] private float _slotSize = 3;
        [SerializeField] private float _randomOffsetFraction = 1f / 6;
        [SerializeField] private float _groupSpacing = 3;
        [SerializeField] private float _emptySlotPercentage = 0.5f;

        public float SlotSize => _slotSize;
        public float RandomOffsetFraction => _randomOffsetFraction;
        public float GroupSpacing => _groupSpacing;
        public float EmptySlotPercentage => _emptySlotPercentage;
    }
}