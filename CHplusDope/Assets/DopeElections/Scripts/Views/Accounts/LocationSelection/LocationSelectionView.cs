using System.Linq;
using AnimatedObjects;
using CameraSystems;
using DopeElections.Answer;
using DopeElections.HotAirBalloon;
using DopeElections.Localizations;
using DopeElections.Planets;
using DopeElections.Progression;
using DopeElections.Tutorials;
using DopeElections.Users;
using Essentials;
using Localizator;
using Progression;
using StateMachines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Accounts
{
    public class LocationSelectionView : AccountView
    {
        public override NamespacedKey Id => AccountViewId.LocationSelection;

        [Header("Prefab References")] [SerializeField]
        private Button _backButton = null;

        [SerializeField] private Color _okColor = Color.white;
        [SerializeField] private Color _badColor = Color.red;
        [SerializeField] private GameObject _zipNotFoundPanel = null;
        [SerializeField] private Graphic _zipTextGraphic = null;
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private PoppablePanelController _confirmFrame = null;
        [SerializeField] private SelectionDisplayFrame _selectionDisplay = null;
        [SerializeField] private InstructionPanel _instructionPanel = null;
        [SerializeField] private Button _approachConfirmationButton = null;
        [SerializeField] private CameraTransformation _cameraTransformation = default;
        [SerializeField] private CameraSystem _cameraSystem = null;

        [Header("Input Fields")] [SerializeField]
        private TMP_InputField _zipInputField = null;

        [SerializeField] private TMP_Dropdown _cantonDropdown = null;
        [SerializeField] private TMP_Dropdown _constituencyDropdown = null;

        [Header("Texts")] [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private LocalizedText _zipPlaceholderText = null;
        [SerializeField] private LocalizedText _cantonPlaceholderText = null;
        [SerializeField] private LocalizedText _constituencyPlaceholderText = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private LocalizedText _orText = null;
        [SerializeField] private LocalizedText _zipNotFoundText = null;

        [Header("Animated Objects")] [SerializeField]
        private ToggleableObjectController _titleFrame = null;

        [SerializeField] private ToggleableObjectController[] _formEntries = null;

        [Header("Scene References")] [SerializeField]
        private Transform _environmentAnchor = null;

        [SerializeField] private Transform _balloonAnchor = null;
        [SerializeField] private Transform _planetAnchor = null;
        [SerializeField] private HotAirBalloonController _balloon = null;
        [SerializeField] private PlanetController _planet = null;

        public HotAirBalloonController Balloon => _balloon;
        public PlanetController Planet => _planet;

        private SelectionDisplayFrame SelectionDisplay => _selectionDisplay;

        private ZipTable _zipTable;
        private Canton[] _cantons;
        private Constituency[] _constituencies;

        private int _previousCantonId;
        private int _previousConstituencyId;
        private int _previousElectionId;

        private Canton _canton;
        private Constituency _constituency;

        private bool _appearWithConfirmation = false;
        private bool _disappearFancy = false;

        #region View Control

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _backButton.onClick.AddListener(Back);

            _zipInputField.onValueChanged.AddListener(OnZipChanged);
            _zipInputField.onSubmit.AddListener(OnZipChanged);
            _cantonDropdown.onValueChanged.AddListener(OnCantonSelected);
            _constituencyDropdown.onValueChanged.AddListener(OnConstituencySelected);
            _confirmButton.onClick.AddListener(Confirm);
            _approachConfirmationButton.onClick.AddListener(PlayFancyAppear);

            _titleText.key = LKey.Views.LocationSelection.Title;
            _zipPlaceholderText.key = LKey.Views.LocationSelection.Zip;
            _cantonPlaceholderText.key = LKey.Views.LocationSelection.Canton;
            _constituencyPlaceholderText.key = LKey.Views.LocationSelection.Constituency;
            _confirmText.key = LKey.Views.LocationSelection.Confirm;
            _orText.key = LKey.Views.LocationSelection.Or;
            _instructionPanel.Text.key = LKey.Views.LocationSelection.Instruction;
            _zipNotFoundText.key = LKey.Views.LocationSelection.ZipNotFound;

            _zipTable = ZipTable.Load();

            Balloon.gameObject.SetActive(false);

            _environmentAnchor.gameObject.SetActive(false);
        }

        public void Unload()
        {
            Planet.transform.SetParent(_planetAnchor, false);
            Planet.transform.Reset();
            _environmentAnchor.gameObject.SetActive(false);
            Balloon.gameObject.SetActive(false);
        }

        protected override StateChangePromise PrepareOpen()
        {
            var appearFancy = DopeElectionsApp.Instance.User.CantonId <= 0;
            _appearWithConfirmation = appearFancy;
            if (!appearFancy) return null;

            gameObject.SetActive(false);

            var promise = new StateChangePromise();
            EditorController.PlayJourneyToPlanetCinematic(() => { promise.Fulfill(); });
            return promise;
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            if (Views.BlackMask.Alpha > 0)
            {
                Views.BlackMask.FadeToClear();
            }

            Views.BlackMask.BlockInteractions(false);

            AttachPlayerToAnchor(Balloon.CharacterAnchor);

            _environmentAnchor.gameObject.SetActive(true);

            Planet.ShowImmediate();
            Planet.CloudLayer.HideImmediate();

            if (!Balloon.gameObject.activeSelf) Balloon.gameObject.SetActive(true);

            _previousCantonId = User.CantonId;
            _previousConstituencyId = User.ConstituencyId;
            _previousElectionId = User.ElectionId;

            var canton = User.GetCanton();
            var constituency = User.GetConstituency();

            UpdateCantonDropdown(canton);
            UpdateConstituencyDropdown(canton, constituency);

            _canton = canton;
            _constituency = constituency;

            if (canton != null && constituency != null) _confirmFrame.Show();
            else _confirmFrame.HideImmediate();

            _zipNotFoundPanel.SetActive(false);

            ApplySelection(canton, constituency);

            if (_appearWithConfirmation) ShowApproachConfirmation();
            else PlayNormalAppear();

            PlayerController.PlayIdleAnimation();

            _disappearFancy = false;
        }

        protected override StateChangePromise PrepareClose()
        {
            if (!_disappearFancy) return null;
            var result = new StateChangePromise();
            PlayFancyDisappear().Then(() => result.Fulfill());
            return result;
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (!_disappearFancy)
            {
                _environmentAnchor.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Appearing and Disappearing

        private void PlayNormalAppear()
        {
            AttachTransformToAnchor(Balloon.transform, _balloonAnchor);
            AttachTransformToAnchor(Planet.transform, _planetAnchor);
            Planet.CloudLayer.ShowImmediate();
            Balloon.PlayIdleAnimation();

            _titleFrame.Show();
            for (var i = 0; i < _formEntries.Length; i++)
            {
                var entry = _formEntries[i];
                entry.Show();
            }

            _instructionPanel.Show();
            _approachConfirmationButton.gameObject.SetActive(false);

            _cameraSystem.CurrentTransform = _cameraTransformation;
        }

        private void PrepareFancyAppear()
        {
            MoveTransformToAnchor(Balloon.transform, _balloonAnchor);
            MoveTransformToAnchor(Planet.transform, _planetAnchor);

            _titleFrame.HideImmediate();
            for (var i = 0; i < _formEntries.Length; i++)
            {
                var entry = _formEntries[i];
                entry.HideImmediate();
            }

            _instructionPanel.HideImmediate();
            Balloon.PlayIdleAnimation(3f);

            _titleFrame.Show();
            _approachConfirmationButton.gameObject.SetActive(true);
        }

        private void PlayFancyAppear()
        {
            const float delay = 0.5f;
            var interval = 0.05f;
            for (var i = 0; i < _formEntries.Length; i++)
            {
                var entry = _formEntries[i];
                entry.HideImmediate();
                entry.Show(i * interval + delay);
            }

            _instructionPanel.Show(interval * (_formEntries.Length + 1) + delay);
            Planet.CloudLayer.Show();

            _approachConfirmationButton.gameObject.SetActive(false);
        }

        private TransitionState PlayFancyDisappear()
        {
            const float delay = 0;
            Views.BlackMask.BlockInteractions(true);
            _titleFrame.Hide(delay);
            var interval = 0.05f;
            for (var i = 0; i < _formEntries.Length; i++)
            {
                var entry = _formEntries[i];
                entry.Hide(i * interval + delay);
            }

            return _instructionPanel.Hide(interval * (_formEntries.Length + 1) + delay);
        }

        #endregion

        #region Actions

        private void ShowApproachConfirmation()
        {
            PrepareFancyAppear();
        }

        private void Confirm()
        {
            Save();
            User.ReloadQuestionnaire();

            var openingComicProgress = User.UserJourney.GetEntry(UserJourneyStepId.OpeningComic);
            if (openingComicProgress.State != ProgressEntry.ProgressState.Completed)
            {
                _disappearFancy = true;
                Views.ComicSequenceView.Open();
                return;
            }

            EditorController.Quit();
        }

        private void Back()
        {
            Clear();

            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() =>
            {
                if (User.UserJourney.GetEntry(UserJourneyStepId.CharacterCreation).State ==
                    ProgressEntry.ProgressState.Completed)
                {
                    DopeElectionsRouter.GoToMainMenu();
                }
                else
                {
                    Views.FaceSelectionView.Open(User);
                }
            });
        }

        private void Save()
        {
            User.Save();
        }

        private void Clear()
        {
            User.CantonId = _previousCantonId;
            User.ConstituencyId = _previousConstituencyId;
            User.ElectionId = _previousElectionId;
        }

        #endregion

        #region Event Listeners

        private void OnZipChanged(string zip)
        {
            if (zip.Length < 4 && _zipInputField.isFocused) return;

            var canton = _zipTable.TryGetCanton(zip, out var cantonMatch) ? cantonMatch : null;
            var cantonId = canton != null ? canton.id : 0;

            var constituencies = DopeElectionsApp.Instance.Assets
                .GetAssets<Constituency>(c => c.cantonId == cantonId && c.type != Constituency.Type.Canton).ToList();
            var constituency = constituencies.Count == 1 ? constituencies.FirstOrDefault() : null;

            var isValid = canton != null && constituencies.Count > 0;

            _zipTextGraphic.color = isValid ? _okColor : _badColor;
            _zipNotFoundPanel.SetActive(!isValid);

            if (!isValid || zip.Length == 0) return;

            User.CantonId = canton.id;
            User.ConstituencyId = constituency != null ? constituency.id : 0;
            User.ElectionId = 0;

            _canton = canton;
            _constituency = constituency;

            UpdateCantonDropdown(canton);
            UpdateConstituencyDropdown(canton, constituency);

            ApplySelection(canton, constituency);
        }

        private void OnCantonSelected(int index)
        {
            if (_cantons == null) return;
            var canton = index >= 0 ? _cantons[index] : null;
            var constituency = _constituency != null && canton != null && _constituency.cantonId == canton.id
                ? _constituency
                : null;
            User.CantonId = canton != null ? canton.id : 0;
            User.ConstituencyId = constituency != null ? constituency.id : 0;
            User.ElectionId = 0;
            _canton = canton;
            _constituency = constituency;

            _zipInputField.SetTextWithoutNotify("");
            _zipTextGraphic.color = _okColor;

            _cantonPlaceholderText.gameObject.SetActive(canton == null);

            UpdateConstituencyDropdown(canton, constituency);
            _zipNotFoundPanel.SetActive(_constituencies == null || _constituencies.Length == 0);

            ApplySelection(canton, constituency);
        }

        private void OnConstituencySelected(int index)
        {
            if (_constituencies == null) return;
            var constituency = index >= 0 ? _constituencies[index] : null;
            User.ConstituencyId = constituency != null ? constituency.id : 0;
            User.ElectionId = 0;
            _constituency = constituency;

            _constituencyPlaceholderText.gameObject.SetActive(constituency == null);

            _zipInputField.SetTextWithoutNotify("");
            _zipTextGraphic.color = _okColor;

            ApplySelection(_canton, _constituency);
        }

        #endregion

        #region Logic

        private void ApplySelection(Canton canton, Constituency constituency)
        {
            SelectionDisplay.Set(canton, constituency);
            _instructionPanel.Text.key = canton != null && constituency != null
                ? LKey.Views.LocationSelection.YouVoteHere
                : LKey.Views.LocationSelection.Instruction;
            _instructionPanel.Text.SetVariable("constituency", constituency != null ? constituency.name : null);
            UpdateConfirmButton();
        }

        private void UpdateCantonDropdown(Canton canton)
        {
            var constituencies = DopeElectionsApp.Instance.Assets.GetAssets<Constituency>();
            var cantons = DopeElectionsApp.Instance.Assets.GetAssets<Canton>()
                .Where(ca => constituencies.Any(co => co.cantonId == ca.id))
                .OrderBy(c => c.name)
                .ToArray();
            _cantonDropdown.options = cantons
                .Select(c => new TMP_Dropdown.OptionData(c.name))
                .ToList();
            _cantonDropdown.SetValueWithoutNotify(canton != null ? cantons.IndexOf(canton) : -1);
            _cantons = cantons;

            _cantonPlaceholderText.gameObject.SetActive(canton == null);
        }

        private void UpdateConstituencyDropdown(Canton canton, Constituency constituency)
        {
            if (canton == null && _cantons != null) canton = _cantons.FirstOrDefault();
            _constituencyDropdown.gameObject.SetActive(canton != null);
            if (canton == null)
            {
                _constituencies = null;
                return;
            }

            var constituencies = DopeElectionsApp.Instance.Assets
                .GetAssets<Constituency>(c => c.cantonId == canton.id && c.type != Constituency.Type.Canton)
                .OrderBy(c => c.name).ToList();

            _constituencyDropdown.options = constituencies
                .Select(c => new TMP_Dropdown.OptionData(c.name))
                .ToList();
            _constituencyDropdown.SetValueWithoutNotify(constituency != null && constituencies.Contains(constituency)
                ? constituencies.IndexOf(constituency)
                : -1);
            _constituencies = constituencies.ToArray();

            _constituencyPlaceholderText.gameObject.SetActive(constituency == null);
        }

        private void UpdateConfirmButton()
        {
            var done = _canton != null && _constituency != null;
            if (done == _confirmFrame.IsVisible) return;
            if (done) _confirmFrame.Show();
            else _confirmFrame.Hide();
        }

        #endregion
    }
}