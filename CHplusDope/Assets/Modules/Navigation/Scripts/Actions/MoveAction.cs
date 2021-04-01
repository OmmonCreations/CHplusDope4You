using UnityEngine;

namespace Navigation
{
    public class MoveAction : NavigationAction
    {
        public MoveAction(Vector2Int @from, Vector2Int to, float time) : base(@from, to, time)
        {
        }
    }
}