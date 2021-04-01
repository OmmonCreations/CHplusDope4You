using UnityEngine;

namespace Navigation
{
    public interface INavigationAction
    {
        Vector2Int From { get; set; }
        Vector2Int To { get; set; }
        float Time { get; set; }

        void Start(INavigationAgent agent);
        void Stop(INavigationAgent agent);
    }
}