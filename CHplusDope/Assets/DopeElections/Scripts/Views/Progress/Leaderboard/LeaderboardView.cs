using AnimatedObjects;
using CameraSystems;
using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Tutorials;
using Essentials;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Progress
{
    public class LeaderboardView : ProgressView, IView, IView<Candidate>
    {
        public override NamespacedKey Id => ProgressViewId.Leaderboard;

        [Header("Prefab References")] [SerializeField]
        private LocalizedText _titleText = null;

        [SerializeField] private InstructionPanel _instructionPanel = null;
        [SerializeField] private ToggleableObjectController[] _leaderboardAnimationControllers = null;

        [SerializeField] private Button _backButton = null;
        [SerializeField] private ToggleableObjectController _titleAnimationController = null;

        [Header("Scene References")] [SerializeField]
        private LeaderboardController _leaderboard = null;

        [SerializeField] private Transform _cameraAnchor = null;
        [Header("Settings")] [SerializeField] private float _cameraFov = 30;
        [SerializeField] private AnimationCurve _cameraTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private LeaderboardController Leaderboard => _leaderboard;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _backButton.onClick.AddListener(GoBack);
            _titleText.key = LKey.Views.Leaderboard.Title;
            _instructionPanel.Text.key = LKey.Views.Leaderboard.Instruction;

            _titleAnimationController.HideImmediate();
            _instructionPanel.HideImmediate();
            foreach (var c in _leaderboardAnimationControllers) c.HideImmediate();

            Leaderboard.Initialize();
            Leaderboard.Interactable = false;
            Leaderboard.Selected += OnCandidateSelected;
        }

        public new void Open() => base.Open();

        public void Open(Candidate selected)
        {
            //TODO select candidate in leaderboard
            base.Open();
        }

        protected override StateChangePromise PrepareOpen()
        {
            var promise = new StateChangePromise();
            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();
            Views.CameraSystem.Transition(CreateCameraTransformation(), 0.8f, _cameraTransitionCurve).Then(promise.Fulfill);
            return promise;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _titleAnimationController.Show();
            _instructionPanel.Show();
            for (var i = 0; i < _leaderboardAnimationControllers.Length; i++)
            {
                _leaderboardAnimationControllers[i].Show(0.1f + i * 0.05f);
            }
            Views.BlackMask.BlockInteractions(false);
            Leaderboard.Interactable = true;
        }

        protected override StateChangePromise PrepareClose()
        {
            Leaderboard.Interactable = false;
            var promise = new StateChangePromise();
            _titleAnimationController.Hide();
            foreach (var c in _leaderboardAnimationControllers) c.Hide();
            _instructionPanel.Hide().Then(promise.Fulfill);
            return promise;
        }

        public void UpdateLeaderboard()
        {
            Leaderboard.UpdateCollection();
            Leaderboard.UpdateControls();
        }

        private void GoBack()
        {
            Views.ProgressionView.Open(true);
        }

        private void OnCandidateSelected(Candidate candidate)
        {
            DopeElectionsApp.Instance.Views.CandidateView.Open(candidate);
        }

        public CameraTransformation CreateCameraTransformation()
        {
            var cameraAnchor = _cameraAnchor;
            var parent = Views.CameraSystem.transform.parent;
            var cameraAnchorPosition = parent.InverseTransformPoint(cameraAnchor.position);
            var leaderboardPosition = parent.InverseTransformPoint(Leaderboard.CameraTarget.position);
            var cameraAnchorUp = parent.InverseTransformDirection(cameraAnchor.up);

            var delta = leaderboardPosition - cameraAnchorPosition;
            var eulerAngles = Quaternion.LookRotation(delta.normalized, cameraAnchorUp).eulerAngles;

            return new CameraTransformation
            {
                position = leaderboardPosition,
                distance = delta.magnitude,
                eulerAngles = eulerAngles,
                fov = _cameraFov
            };
        }
    }
}