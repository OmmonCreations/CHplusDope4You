using UnityEngine;

namespace Navigation
{
    public class IdleAction : NavigationAction
    {
        public IdleAction(Vector2Int position, float time) : base(position, position, time)
        {
        }
    }
}