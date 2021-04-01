using System.Linq;
using UnityEngine;


namespace SpeechBubbles
{
    public class SpeechBubbleLayer : MonoBehaviour, ISpeechBubbleSource
    {
        [Header("Prefab References")] [SerializeField]
        private RectTransform _speechBubblesArea = null;

        [SerializeField] private SpeechBubbleCollection _collection = null;
        [SerializeField] private Vector4 _padding = Vector4.zero;

        [Header("Scene References")] [SerializeField]
        private Camera _camera = null;

        public RectTransform SpeechBubblesArea => _speechBubblesArea;

        public Vector4 Padding
        {
            get => _padding;
            set => _padding = value;
        }

        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        private void OnEnable()
        {
            if (_speechBubblesArea) _speechBubblesArea.pivot = Vector2.up;
        }

        public SpeechBubbleController ShowSpeechBubble<T>(T speechBubble) where T : SpeechBubble
        {
            var prefab = _collection.GetPrefab<T, SpeechBubbleController<T>>();
            return ShowSpeechBubble(prefab, speechBubble);
        }

        public T2 ShowSpeechBubble<T, T2>(T speechBubble) where T : SpeechBubble where T2 : SpeechBubbleController<T>
        {
            var prefab = _collection.GetPrefab<T,T2>();
            if (!prefab)
            {
                Debug.LogError("No SpeechBubbleController of type " + typeof(T2).Name + " found.");
                return null;
            }

            return CreateSpeechBubble(prefab, speechBubble);
        }

        public T2 ShowSpeechBubble<T, T2>(T2 prefab, T speechBubble)
            where T : SpeechBubble where T2 : SpeechBubbleController<T>
        {
            return CreateSpeechBubble(prefab, speechBubble);
        }

        private T2 CreateSpeechBubble<T, T2>(T2 prefab, T speechBubble)
            where T : SpeechBubble where T2 : SpeechBubbleController<T>
        {
            var instanceObject = Instantiate(prefab.gameObject, _speechBubblesArea, false);
            var instance = instanceObject.GetComponent<T2>();
            instance.Initialize(this, speechBubble);
            instanceObject.SetActive(true);
            return instance;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_speechBubblesArea) _speechBubblesArea.pivot = Vector2.up;
        }
#endif
    }
}