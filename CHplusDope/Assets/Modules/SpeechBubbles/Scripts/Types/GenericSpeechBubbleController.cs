using UnityEngine;

namespace SpeechBubbles
{
    public class GenericSpeechBubbleController : SpeechBubbleController<GenericSpeechBubble>
    {
        protected override Vector2 PreferredSize => MinSize;
    }
}