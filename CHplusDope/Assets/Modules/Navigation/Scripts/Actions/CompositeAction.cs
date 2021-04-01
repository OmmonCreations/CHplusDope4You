using System.Linq;
using UnityEngine;

namespace Navigation
{
    public class CompositeAction : INavigationAction, ICompilableAction
    {
        public Vector2Int From { get; set; }
        public Vector2Int To { get; set; }
        public float Time { get; set; }

        public INavigationAction[] Actions { get; }

        public CompositeAction(params INavigationAction[] actions)
        {
            From = actions[0].From;
            To = actions[actions.Length - 1].To;
            Time = actions.Sum(a => a.Time);
            Actions = actions;
        }

        public void Start(INavigationAgent agent)
        {
            Debug.LogWarning("Path has not been compiled!");
        }

        public void Stop(INavigationAgent agent)
        {
            Debug.LogWarning("Path has not been compiled!");
        }

        public void Compile(RawPath path, int ownIndex)
        {
            var actions = path.Actions;
            var ownActions = Actions;

            actions.RemoveAt(ownIndex);

            for (var i = 0; i < ownActions.Length; i++)
            {
                var action = ownActions[i];
                actions.Insert(ownIndex + i, action);
            }

            // compile action that was insert instead of this one, remaining ones get compiled by outer loop
            if (actions[ownIndex] is ICompilableAction compilableAction)
            {
                compilableAction.Compile(path, ownIndex);
            }
        }

        public override string ToString() => $"{From} to {To}\n" + string.Join("\n", Actions.Select(a => "- " + a));
    }
}