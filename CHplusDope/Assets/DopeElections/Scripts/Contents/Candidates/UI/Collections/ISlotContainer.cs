using MobileInputs;
using UnityEngine;

namespace DopeElections.Candidates
{
    public interface ISlotContainer
    {
        InteractionSystem InteractionSystem { get; }
        float SlotSize { get; }
        Vector2 GetSlotVector(Vector2Int slotPosition);
    }
}