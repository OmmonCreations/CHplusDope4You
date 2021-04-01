using UnityEngine;

namespace Navigation
{
    public abstract class NavigationAction : INavigationAction
    {
        public delegate void ActionEvent(INavigationAgent agent);

        public event ActionEvent Started = delegate { };
        public event ActionEvent Stopped = delegate { };

        public Vector2Int From { get; set; }
        public Vector2Int To { get; set; }
        public float Time { get; set; }

        protected NavigationAction(Vector2Int from, Vector2Int to, float time)
        {
            From = from;
            To = to;
            Time = time;
        }

        public void Start(INavigationAgent agent)
        {
            OnStarted(agent);
            Started(agent);
        }

        protected virtual void OnStarted(INavigationAgent agent)
        {
        }

        public void Stop(INavigationAgent agent)
        {
            OnStopped(agent);
            Stopped(agent);
        }

        protected virtual void OnStopped(INavigationAgent agent)
        {
        }

        public override string ToString() => $"{From} to {To} \"" + GetType().Name + $"\" ({Time}s)";
    }
}