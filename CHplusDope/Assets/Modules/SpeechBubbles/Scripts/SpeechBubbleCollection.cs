using System.Linq;
using UnityEngine;

namespace SpeechBubbles
{
    [CreateAssetMenu(fileName = "SpeechBubbleCollection", menuName = "Speech Bubbles/Speech Bubble Collection")]
    public class SpeechBubbleCollection : ScriptableObject
    {
        [SerializeField] private SpeechBubbleController[] _speechBubblePrefabs = null;

        internal T2 GetPrefab<T, T2>() where T : SpeechBubble where T2 : SpeechBubbleController<T>
        {
            return _speechBubblePrefabs.OfType<T2>().FirstOrDefault();
        }
    }
}