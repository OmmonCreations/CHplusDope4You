using System;
using Localizator;
using StateMachines;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Races
{
    public class RaceOverlayController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [Header("Top Area")] [SerializeField] private Button _backButton = null;
        [SerializeField] private LocalizedText _categoryText = null;
        [SerializeField] private Button _helpButton = null;
        [SerializeField] private RaceProgressDisplayController _progressDisplay = null;
        [SerializeField] private float _contractedY = 100;
        [SerializeField] private float _expandedY = -25;
        [SerializeField] private AnimationCurve _expandPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _contractPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);

        #endregion

        #region Private Fields

        private bool _visible;
        private LocalizationKey _title;
        private Action _backAction;
        private Action _openHelpAction;

        #endregion

        #region Auto Properties

        public RectTransform RectTransform => _rectTransform;
        public RaceProgressDisplayController ProgressDisplay => _progressDisplay;
        private StateMachine StateMachine => _stateMachine;
        internal float ContractedY => _contractedY;
        internal float ExpandedY => _expandedY;
        internal AnimationCurve ExpandPositionCurve => _expandPositionCurve;
        internal AnimationCurve ContractPositionCurve => _contractPositionCurve;

        #endregion

        #region Properties

        public Action BackAction
        {
            get => _backAction;
            set => ApplyBackAction(value);
        }

        public Action OpenHelpAction
        {
            get => _openHelpAction;
            set => ApplyOpenHelpAction(value);
        }

        public LocalizationKey Title
        {
            get => _title;
            set => ApplyTitle(value);
        }

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set { _canvasGroup.interactable = value; }
        }

        #endregion

        #region Unity Control

        private void Awake()
        {
            _backButton.onClick.AddListener(Back);
            _helpButton.onClick.AddListener(OpenHelp);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        #endregion

        #region Actions

        private void Back()
        {
            if (BackAction != null) BackAction();
        }

        private void OpenHelp()
        {
            if (OpenHelpAction != null) OpenHelpAction();
        }

        #endregion

        #region Data Modifiers

        private void ApplyTitle(LocalizationKey title)
        {
            _title = title;
            _categoryText.key = title;
        }

        private void ApplyBackAction(Action action)
        {
            _backAction = action;
            _backButton.gameObject.SetActive(action != null);
        }

        private void ApplyOpenHelpAction(Action action)
        {
            _openHelpAction = action;
            _helpButton.gameObject.SetActive(action != null);
        }

        #endregion

        #region Public API

        public void Show() => Show(true);
        public void Hide() => Show(false);

        public void Show(bool show)
        {
            if (show == _visible) return;
            _visible = show;
            if (!RectTransform.gameObject.activeSelf) RectTransform.gameObject.SetActive(true);
            StateMachine.State = show ? (State) new ExpandOverlayState(this) : new ContractOverlayState(this);
        }

        public void ShowImmediate() => ShowImmediate(true);
        public void HideImmediate() => ShowImmediate(false);

        public void ShowImmediate(bool show)
        {
            _visible = show;
            RectTransform.gameObject.SetActive(show);
            Interactable = show;
            Alpha = show ? 1 : 0;
        }

        #endregion
    }
}