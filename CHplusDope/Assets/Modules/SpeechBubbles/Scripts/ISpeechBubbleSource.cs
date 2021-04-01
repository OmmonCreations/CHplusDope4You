namespace SpeechBubbles
{
    public interface ISpeechBubbleSource
    {
        T2 ShowSpeechBubble<T,T2>(T speechBubble) where T : SpeechBubble where T2 : SpeechBubbleController<T>;
        T2 ShowSpeechBubble<T,T2>(T2 prefab, T speechBubble) where T : SpeechBubble where T2 : SpeechBubbleController<T>;
    }
}