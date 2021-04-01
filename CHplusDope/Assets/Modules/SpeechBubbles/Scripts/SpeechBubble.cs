using StateMachines;
using UnityEngine;

namespace SpeechBubbles
{
    public abstract class SpeechBubble
    {
        public ITargetable Source { get; }
        public Vector2 PreferredTail { get; }
        public float Time { get; }

        public SpeechBubble(ITargetable source) : this(source, new Vector2(0.2f, 0))
        {
        }

        public SpeechBubble(ITargetable source, Vector2 preferredTail, float time = 0)
        {
            Source = source;
            PreferredTail = preferredTail;
            Time = time;
        }
    }
}