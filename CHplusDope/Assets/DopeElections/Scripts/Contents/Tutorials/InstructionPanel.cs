using AnimatedObjects;
using Localizator;
using StateMachines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.Tutorials
{
    public class InstructionPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private ToggleablePanelController _panelController = null;
        [SerializeField] private Button _button = null;

        [SerializeField] private LocalizedText _text = null;

        public ToggleablePanelController PanelController => _panelController;

        public bool Interactable
        {
            get => _button.enabled;
            set => _button.enabled = value;
        }

        private float Alpha
        {
            get => _canvasGroup.alpha;
            set
            {
                _canvasGroup.alpha = value;
                if (value <= 0 && gameObject.activeSelf) gameObject.SetActive(false);
                if (value > 0 && !gameObject.activeSelf) gameObject.SetActive(true);
            }
        }

        public UnityEvent onClick => _button.onClick;

        public LocalizedText Text => _text;

        public RectTransform RectTransform => _rectTransform;

        protected void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        private void Update()
        {
            _stateMachine.Run();
        }

        public TransitionState Show(bool show, float delay) => PanelController.Show(show, delay);
        public TransitionState Show(float delay = 0) => PanelController.Show(delay);
        public TransitionState Hide(float delay = 0) => PanelController.Hide(delay);

        public void ShowImmediate(bool show) => PanelController.ShowImmediate(show);
        public void ShowImmediate() => PanelController.ShowImmediate();
        public void HideImmediate() => PanelController.HideImmediate();
    }
}