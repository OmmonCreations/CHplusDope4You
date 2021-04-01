using System.Linq;
using AnimatedObjects;
using DopeElections.ExtraInfoTalkers;
using DopeElections.Progression;
using Localizator;
using MobileInputs;
using Progression;
using Progression.UI;
using StateMachines;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DopeElections.Progress
{
    public class PlanetProgressStepController : ProgressEntryController, IPointerDownListener, IPointerUpListener,
        ITapListener
    {
        [SerializeField] private Transform _canvasAnchor = null;
        [SerializeField] private Transform _playerAnchor = null;
        [SerializeField] private ToggleableObjectController _labelVisilibityController = null;
        [SerializeField] private ToggleableObjectController _labelEnlargeController = null;
        [SerializeField] private WiggleableObjectController _labelWiggleController = null;
        [SerializeField] private ProgressStepMeshController _meshController = null;
        [SerializeField] private LocalizedText _labelText = null;
        [SerializeField] private Collider _collider = null;

        [SerializeField] private Image _mainImage = null;
        [SerializeField] private Image _backgroundImage = null;
        [SerializeField] private Graphic[] _coloredGraphics = null;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _lockedColor = Color.white;

        [SerializeField] private Transform _canvasTransform = null;
        [SerializeField] private Transform _canvasAnchorTop = null;
        [SerializeField] private Transform _canvasAnchorBottom = null;

        [SerializeField] private Transform _talkerAnchor = null;

        [Header("Sprites")] [SerializeField] private Sprite _mainNormal = null;
        [SerializeField] private Sprite _mainLocked = null;
        [SerializeField] private Sprite _backgroundNormal = null;
        [SerializeField] private Sprite _backgroundLocked = null;

        private IExtraInfoEntry _extraInfoEntry;
        private ExtraInfoTalkerController _extraInfoTalker;

        private ProgressStepMeshController MeshController => _meshController;

        private PlanetProgressController PlanetController { get; set; }
        private Transform CameraTransform { get; set; }

        public Transform PlayerAnchor => _playerAnchor;
        public ExtraInfoTalkerController ExtraInfoTalker => _extraInfoTalker;

        public IExtraInfoEntry ExtraInfo
        {
            get => _extraInfoEntry;
            set => ApplyExtraInfo(value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var planetController = TreeController as PlanetProgressController;
            var camera = planetController ? planetController.Camera : null;
            PlanetController = planetController;
            CameraTransform = camera ? camera.transform : null;
            _labelEnlargeController.HideImmediate();
            _canvasTransform.SetParent(Index % 2 == 0 ? _canvasAnchorTop : _canvasAnchorBottom, false);

            UpdateState();
        }

        private void LateUpdate()
        {
            if (CameraTransform) _canvasAnchor.rotation = CameraTransform.rotation;
        }

        protected override void OnLabelChanged(LocalizationKey label)
        {
            base.OnLabelChanged(label);
            _labelText.key = label;
        }

        protected override void OnShowLabelImmediate(bool show)
        {
            base.OnShowLabelImmediate(show);
            _labelVisilibityController.ShowImmediate(show);
        }

        protected override void OnStateChanged(string state)
        {
            base.OnStateChanged(state);
            UpdateState();
        }

        protected override void OnInteractableChanged(bool interactable)
        {
            base.OnInteractableChanged(interactable);
            _collider.enabled = interactable;
        }

        private void ApplyExtraInfo(IExtraInfoEntry extraInfo)
        {
            _extraInfoEntry = extraInfo;
            if (_extraInfoTalker) _extraInfoTalker.Remove();
            if (extraInfo != null && extraInfo.State != ProgressEntry.ProgressState.Completed)
            {
                _extraInfoTalker = CreateTalker(extraInfo);
            }
        }

        public void PlayUnlockAnimation()
        {
            MeshController.PlayUnlockAnimation();
        }

        private void UpdateState()
        {
            var state = State;
            var locked = state == ProgressEntry.ProgressState.Locked;
            var color = locked ? _lockedColor : _normalColor;
            _mainImage.sprite = locked ? _mainLocked : _mainNormal;
            _backgroundImage.sprite = locked ? _backgroundLocked : _backgroundNormal;
            foreach (var g in _coloredGraphics) g.color = color;

            MeshController.Locked = locked;
            MeshController.IsActive = state == ProgressEntry.ProgressState.Unlocked;
            MeshController.UpdateIcon(Entry);
        }

        protected override TransitionState CreateLabelTransitionState(bool show, float delay)
        {
            return _labelVisilibityController ? _labelVisilibityController.Show(show, delay) : null;
        }

        public void OnTap(InputAction.CallbackContext context)
        {
            Tap();
        }

        public void Tap()
        {
            if (Entry.State == ProgressEntry.ProgressState.Locked)
            {
                _labelWiggleController.Wiggle();
            }

            onClick.Invoke();
        }

        public void OnPointerDown(InputAction.CallbackContext context)
        {
            _labelEnlargeController.Show();
        }

        public void OnPointerUp(InputAction.CallbackContext context)
        {
            _labelEnlargeController.Hide();
        }

        private ExtraInfoTalkerController CreateTalker(IExtraInfoEntry entry)
        {
            var prefab = DopeElectionsApp.Instance.Assets.GetAsset<ExtraInfoTalkerController>(entry.TalkerId);
            if (!prefab)
            {
                Debug.LogError("Talker prefab with id " + entry.TalkerId + " not found!");
                return null;
            }

            var instanceObject = Instantiate(prefab.gameObject, _talkerAnchor, false);
            var instance = instanceObject.GetComponent<ExtraInfoTalkerController>();
            instance.Initialize(entry);
            return instance;
        }
    }
}