using CameraSystems;
using Localizator;
using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence.Guides
{
    public class TapGuideController : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private RectTransform _tapAreaTransform = null;
        [SerializeField] private RectTransform _labelTransform = null;
        [SerializeField] private LocalizedText _labelText = null;

        private RectTransform _guideArea;

        public GrabSequenceController SequenceController { get; private set; }
        public CameraSystem CameraSystem { get; private set; }
        public LocalizedText Label => _labelText;
        
        private GrabSequencePlayerController PlayerController { get; set; }

        private RectTransform RectTransform => _rectTransform;

        public void Initialize(GrabSequenceController sequenceController)
        {
            SequenceController = sequenceController;
            CameraSystem = sequenceController.CameraSystem;
            _guideArea = transform.parent as RectTransform;
            gameObject.SetActive(false);
        }

        public void Follow(GrabSequencePlayerController playerController)
        {
            PlayerController = playerController;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            var areaSize = 0.85f + (Mathf.Sin(Time.realtimeSinceStartup * 4.425f) + 1) / 2 * 0.3f;
            var labelSize = 0.85f + (Mathf.Sin(Time.realtimeSinceStartup * 3.5f) + 1) / 2 * 0.3f;
            _tapAreaTransform.localScale = Vector3.one * areaSize;
            _labelTransform.localScale = Vector3.one * labelSize;
        }

        private void LateUpdate()
        {
            if (!PlayerController || !_guideArea) return;
            var position = PlayerController.transform.position;
            var screenPoint = CameraSystem.GetScreenPoint(position);
            var relativeScreenPoint = new Vector2(screenPoint.x / Screen.width, 1 - screenPoint.y / Screen.height);
            var rect = _guideArea.rect;
            RectTransform.anchoredPosition = new Vector2(
                relativeScreenPoint.x * rect.width,
                -relativeScreenPoint.y * rect.height
            );
        }
    }
}