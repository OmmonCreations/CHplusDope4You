using StateMachines;
using TMPro;
using UnityEngine;

namespace Progression.UI
{
    public class UIProgressEntryController : ProgressEntryController, ICanvasTargetable
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private GameObject _completedState = null;
        [SerializeField] private GameObject _unlockedState = null;
        [SerializeField] private GameObject _lockedState = null;

        private bool _isHoldInteraction;
        
        public RectTransform RectTransform => _rectTransform;
        public Vector3 Position => RectTransform.position;
        public Quaternion Rotation => RectTransform.rotation;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            // TooltipSource = TreeController.TooltipSource;
        }

        protected override void OnStateChanged(string state)
        {
            base.OnStateChanged(state);
            _completedState.gameObject.SetActive(state == ProgressEntry.ProgressState.Completed);
            _unlockedState.gameObject.SetActive(state == ProgressEntry.ProgressState.Unlocked);
            _lockedState.gameObject.SetActive(state == ProgressEntry.ProgressState.Locked);
        }

        protected override TransitionState CreateLabelTransitionState(bool show, float delay)
        {
            return null;
        }
    }
}