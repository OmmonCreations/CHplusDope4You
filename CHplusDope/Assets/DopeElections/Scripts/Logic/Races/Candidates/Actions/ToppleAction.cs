using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    /// <summary>
    /// A candidate falls to the ground after stumbling
    /// </summary>
    public class ToppleAction : RaceCandidateAction, ICompilableAction
    {
        public ToppleAction(Vector2Int position, float time) : base(position, position, time)
        {
        }

        public void Compile(RawPath path, int ownIndex)
        {
            var actions = path.Actions;
            var remainingFallTime = 1f;
            var totalFallTime = 0f;
            for (var i = ownIndex - 1; i >= 0; i--)
            {
                var action = actions[i];
                if (!(action is MoveAction)) break;
                totalFallTime += action.Time;
                actions[i] = new StumbleAction(action.From, action.To, action.Time, totalFallTime);
                remainingFallTime -= action.Time;
                if (remainingFallTime <= 0) return;
            }

            if (ownIndex < actions.Count - 1 && actions[ownIndex + 1] is MoveAction moveAction)
            {
                actions[ownIndex+1] = new RecoverAction(moveAction.From, moveAction.To, moveAction.Time);
            }
        }
    }
}