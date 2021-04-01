using StateMachines;
using UnityEngine;

namespace SpeechBubbles
{
    public class TextSpeechBubble : SpeechBubble
    {
        public string Text { get; }

        public TextSpeechBubble(ITargetable source, string text) : this(source, text, new Vector2(0.2f, 0))
        {
        }

        public TextSpeechBubble(ITargetable source, string text, float time) : this(source, text, new Vector2(0.2f, 0),
            time)
        {
        }

        public TextSpeechBubble(ITargetable source, string text, Vector2 preferredTail, float time = 0) : base(source,
            preferredTail, time)
        {
            Text = text;
        }
    }
}