using AnimatedObjects;
using CameraSystems;
using DopeElections.Localizations;
using DopeElections.Progression;
using DopeElections.Sounds;
using Effects;
using Essentials;
using FMODSoundInterface;
using Localizator;
using Progression;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Accounts
{
    public class FaceSelectionView : AccountView
    {
        public override NamespacedKey Id => AccountViewId.FaceSelection;

        [Header("Prefab References")] [SerializeField]
        private Button _backButton = null;

        [SerializeField] private SlidablePanelController _titleFrame = null;
        [SerializeField] private LocalizedText _titleText = null;

        [SerializeField] private FaceOptionController[] _options = null;
        [SerializeField] private RectTransform _dropArea = null;
        [SerializeField] private EffectInstance _faceChangedEffect = null;

        [SerializeField] private PoppablePanelController _confirmFrame = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private CameraTransformation _cameraTransformation = default;

        [Header("Scene References")] [SerializeField]
        private CameraSystem _cameraSystem = null;

        [SerializeField] private Transform _environmentAnchor = null;


        [SerializeField] private Transform _playerAnchor = null;
        [SerializeField] private Transform _characterBodyTransform = null;

        public RectTransform DropArea => _dropArea;

        private bool _confirmButtonVisible;
        private ParticleEffectInstance _lastFaceChangeEffect;

        #region Menu Control

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _backButton.onClick.AddListener(Back);

            foreach (var o in _options) o.Alpha = 0;
            _confirmButton.onClick.AddListener(Confirm);
            _confirmButton.gameObject.SetActive(false);
            _titleText.key = LKey.Views.FaceSelection.Title;
            _confirmText.key = LKey.Views.FaceSelection.Confirm;
            _titleFrame.HideImmediate();
            _confirmFrame.HideImmediate();

            foreach (var o in _options)
            {
                o.DropArea = DropArea;
                o.Selected += OnFaceSelected;
            }

            _environmentAnchor.gameObject.SetActive(false);
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            _environmentAnchor.gameObject.SetActive(true);

            _titleFrame.Show();

            _cameraSystem.CurrentTransform = _cameraTransformation;

            const float step = 0.05f;
            for (var i = 0; i < _options.Length; i++)
            {
                var o = _options[i];
                o.FadeIn(0.5f, i * step);
            }

            var faceSelected = User.FaceId != default;
            _confirmButtonVisible = faceSelected;

            Views.BlackMask.BlockInteractions(false);

            if (faceSelected)
            {
                Views.BlackMask.FadeToClear();
                _confirmFrame.Show();
            }
            else
            {
                _confirmFrame.HideImmediate();
            }

            AttachPlayerToAnchor(_playerAnchor);
            PlayerController.ApplyUserConfiguration(User);
            PlayerController.PlayIdleAnimation();
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (_lastFaceChangeEffect) _lastFaceChangeEffect.Remove();
            _environmentAnchor.gameObject.SetActive(false);
        }

        #endregion

        #region Navigation

        private void Confirm()
        {
            User.Save();
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() =>
            {
                if (DopeElectionsApp.Instance.User.UserJourney.GetEntry(UserJourneyStepId.CharacterCreation).State ==
                    ProgressEntry.ProgressState.Completed)
                {
                    DopeElectionsRouter.GoToMainMenu();
                    return;
                }

                Views.LocationSelectionView.Open(User);
            });
        }

        private void Back()
        {
            EditorController.Quit();
        }

        #endregion

        #region Logic

        private void OnFaceSelected(NamespacedKey faceId)
        {
            PlayerController.Face = faceId;
            User.FaceId = faceId;
            PlayFaceChangedEffect();
            if (!_confirmButtonVisible) ShowConfirmButton();
            SoundController.Play(Sound.Sfx.Player.FaceChanged);
        }

        private void PlayFaceChangedEffect()
        {
            if (_lastFaceChangeEffect) _lastFaceChangeEffect.Remove();

            var instanceObject = Instantiate(_faceChangedEffect.gameObject, _characterBodyTransform, false);
            var instance = instanceObject.GetComponent<ParticleEffectInstance>();
            instance.Play();
            _lastFaceChangeEffect = instance;
            instance.Finished += () =>
            {
                if (_lastFaceChangeEffect == instance) _lastFaceChangeEffect = null;
            };
        }

        private void ShowConfirmButton()
        {
            if (_confirmButtonVisible) return;
            _confirmButtonVisible = true;
            _confirmFrame.Show();
        }

        #endregion
    }
}