using System;
using System.Linq;
using AnimatedObjects;
using CameraSystems;
using DopeElections.ExtraInfoTalkers;
using DopeElections.Localizations;
using DopeElections.MainMenus;
using DopeElections.Progression;
using DopeElections.Races;
using DopeElections.Sounds;
using DopeElections.Tutorials;
using DopeElections.Users;
using Essentials;
using FMODSoundInterface;
using Localizator;
using Progression;
using SpeechBubbles;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.Progress
{
    public class ProgressionView : ProgressView, IView, IView<bool>, IView<IRaceProgressEntry>,
        IView<bool, IRaceProgressEntry>, IView<bool, IRaceProgressEntry, bool>
    {
        public override NamespacedKey Id => ProgressViewId.Progress;

        [Header("Prefab References")] [SerializeField]
        private Button _backButton = null;

        [SerializeField] private InstructionPanel _instructionPanel = null;
        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private CameraTransformation _cameraTransformation = default;
        [SerializeField] private AnimationCurve _cameraTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private ToggleableObjectController _titleAnimationController = null;

        [Header("Scene References")] [SerializeField]
        private PlanetProgressController _progressionController = null;

        [SerializeField] private PlanetLoveMobileController _loveMobile = null;
        [SerializeField] private LocalizedText _loveMobileTitle = null;
        [SerializeField] private SpeechBubbleLayer _speechLayer = null;

        public PlanetProgressController ProgressionController => _progressionController;
        private PlanetLoveMobileController LoveMobile => _loveMobile;

        public RaceProgressionTree Tree { get; private set; }

        private bool ResetCamera { get; set; }
        private IRaceProgressEntry Focused { get; set; }
        private bool PlayUnlockAnimation { get; set; }

        #region Basic Menu method

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _titleText.key = LKey.Views.Progression.Title;
            _loveMobileTitle.key = LKey.Views.Progression.Leaderboard;
            _backButton.onClick.AddListener(Back);
            _progressionController.EntrySelected += OnEntrySelected;

            _titleAnimationController.HideImmediate();
            _instructionPanel.HideImmediate();

            ProgressionController.Interactable = false;
            LoveMobile.Initialize(this);
        }

        public new void Open()
        {
            Open(false);
        }

        public void Open(bool resetCamera)
        {
            var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
            if (questionnaire == null)
            {
                Debug.LogError("No questionnaire loaded");
                return;
            }

            var visibleEntries = questionnaire.Progression.Entries.OfType<IRaceProgressEntry>().ToList();
            var focused = visibleEntries
                .Where(e => e.State != ProgressEntry.ProgressState.Completed)
                .DefaultIfEmpty(visibleEntries.Last())
                .First();
            Open(resetCamera, focused);
        }

        public void Open(IRaceProgressEntry focused)
        {
            Open(false, focused);
        }

        public void Open(bool resetCamera, IRaceProgressEntry focused)
        {
            Open(resetCamera, focused, !focused.IsAvailable && focused.Unlockable);
        }

        public void Open(bool resetCamera, IRaceProgressEntry focused, bool playUnlockAnimation)
        {
            ResetCamera = resetCamera;
            Focused = focused;
            PlayUnlockAnimation = playUnlockAnimation;
            base.Open();
        }

        protected override StateChangePromise PrepareOpen()
        {
            var tree = FetchProgressionTree();
            if (tree == null)
            {
                Debug.LogWarning("No election available.");
                return null;
            }

            Tree = tree;

            var lastAvailable = tree.Entries.OfType<IVisibleProgressEntry>().LastOrDefault(e => e.IsAvailable);
            ProgressionController.PlacePlayer(lastAvailable);

            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();

            if (ResetCamera)
            {
                var promise = new StateChangePromise();
                Views.CameraSystem.Transition(_cameraTransformation, 0.8f, _cameraTransitionCurve)
                    .Then(promise.Fulfill);
                return promise;
            }

            return base.PrepareOpen();
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            if (Tree == null) return;

            if (Focused != null)
            {
                var entry = ProgressionController.Controllers.FirstOrDefault(c => c.Entry == Focused);
                if (entry != null) entry.State = Focused.State;
            }

            if (PlayUnlockAnimation)
            {
                PlayFancyOpen();
            }
            else if (Focused != null && !ResetCamera)
            {
                ProgressionController.FocusImmediate(Focused);
            }

            ProgressionController.ShowLabels();

            Views.BlackMask.BlockInteractions(false);

            _titleAnimationController.Show();
            _instructionPanel.Show();

            LoveMobile.EnableMovement();
            ProgressionController.EnableMovement();
            ProgressionController.UpdateState();
            ProgressionController.Interactable = true;

            UpdateHelpBox();
        }

        protected override StateChangePromise PrepareClose()
        {
            ProgressionController.Interactable = false;
            ProgressionController.HideLabels();
            var promise = new StateChangePromise();
            _titleAnimationController.Hide();
            _instructionPanel.Hide().Then(promise.Fulfill);
            return promise;
        }

        #endregion

        #region Logic

        private void PlayFancyOpen()
        {
            var entries = Tree.Entries.OfType<IVisibleProgressEntry>().ToList();
            var entryIndex = entries.IndexOf(Focused);
            var previousIndex = entryIndex - 1;
            if (previousIndex >= 0 && !ResetCamera) ProgressionController.FocusImmediate(entries[previousIndex]);
            ProgressionController.Focus(Focused);
            DopeElectionsApp.Instance.TaskScheduler.RunTaskLater(() => UnlockFocusedStep(Focused), 0.5f);
        }

        private void UnlockFocusedStep(IVisibleProgressEntry entry)
        {
            if (Focused != entry || !IsOpen) return;
            entry.State = ProgressEntry.ProgressState.Unlocked;
            entry.UpdateLabel();
            var controller = ProgressionController.Controllers.FirstOrDefault(c => c.Entry == entry);
            if (!controller) return;
            controller.PlayUnlockAnimation();
            DopeElectionsApp.Instance.TaskScheduler.RunTaskLater(() =>
            {
                if (!controller) return;
                controller.CanShowLabel = true;
                controller.State = entry.State;
                controller.Label = entry.Label;
                controller.ShowLabel(true);
                PlayPlayerJumpAnimation(controller);
            }, 2f);
        }

        private void PlayPlayerJumpAnimation(PlanetProgressStepController step)
        {
            var playerController = ProgressionController.PlayerController;
            var playerTransform = playerController.transform;
            playerTransform.SetParent(null, true);
            playerTransform.SetParent(step.PlayerAnchor, true);
            playerController.JumpTo(Vector3.zero, Quaternion.identity, Vector3.one, 10f)
                .Then(playerController.PlayIdleLookingUpAnimation);
        }

        private RaceProgressionTree FetchProgressionTree()
        {
            var user = DopeElectionsApp.Instance.User;
            return user.Questionnaire.Progression;
        }

        private void OnEntrySelected(PlanetProgressStepController step)
        {
            var entry = step ? step.Entry : null;
            if (entry == null)
            {
                return;
            }

            var extraInfo = step.ExtraInfo;
            var talker = step.ExtraInfoTalker;
            if (extraInfo != null)
            {
                if (entry.State == ProgressEntry.ProgressState.Locked)
                {
                    var localization = Views.Localization;
                    var text = localization.GetString(extraInfo.LockedText);
                    _speechLayer.ShowSpeechBubble(new TextSpeechBubble(talker, text, 1.5f));
                    return;
                }

                if (extraInfo.State != ProgressEntry.ProgressState.Completed && entry.IsAvailable)
                {
                    GoToExtraInfo(extraInfo, talker);
                    return;
                }
            }

            if (!entry.IsAvailable) return;

            switch (entry)
            {
                case RaceCategoryProgressEntry categoryEntry:
                    GoToRace(categoryEntry);
                    break;
                case RaceTeamEntry teamEntry:
                    GoToTeamSelection(teamEntry);
                    break;
            }
        }

        #endregion

        #region Navigation method

        private void Back()
        {
            Views.BlackMask.FadeToBlack(() => DopeElectionsRouter.GoToMainMenu());
        }

        public void GoToLeaderboard()
        {
            LoveMobile.DisableMovement();
            ProgressionController.DisableMovement();
            Views.LeaderboardView.Open();
        }

        private void GoToExtraInfo(IExtraInfoEntry entry, ExtraInfoTalkerController talker)
        {
            Views.ExtraInfoView.Open(entry, talker);
        }

        private void GoToRace(RaceCategoryProgressEntry entry)
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => GoToRaceImmediate(entry));
        }

        private void GoToRaceImmediate(RaceCategoryProgressEntry entry)
        {
            var context = new RaceContext(Tree, entry);

            if (entry.State == ProgressEntry.ProgressState.Completed && entry.Configuration != 0)
            {
                context.CreateRace();
                context.Race.ApplyFinishedState();
                DopeElectionsRouter.GoToRaceResult(context, false);
            }
            else if (entry.Configuration != 0)
            {
                DopeElectionsRouter.GoToRace(context);
            }
            else
            {
                DopeElectionsRouter.GoToRaceCategorySelection(context);
            }
        }

        private void GoToTeamSelection(RaceTeamEntry entry)
        {
        }

        private void GoToTeamCreation()
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => DopeElectionsRouter.GoToMainMenu(MainMenuViewId.Team));
        }

        #endregion

        #region Helpers

        private void UpdateHelpBox()
        {
            var allEntries = _progressionController.Tree.Entries.OfType<IVisibleProgressEntry>().ToList();
            var activeEntry = allEntries
                .Where(e => e.State == ProgressEntry.ProgressState.Unlocked)
                .DefaultIfEmpty(allEntries.FirstOrDefault(e => e.State == ProgressEntry.ProgressState.Locked) ??
                                allEntries.FirstOrDefault())
                .FirstOrDefault();

            _instructionPanel.Text.key = activeEntry != null ? activeEntry.HelpText : default;
            _instructionPanel.gameObject.SetActive(_instructionPanel.Text.key != default);
            _instructionPanel.onClick.RemoveAllListeners();
            _instructionPanel.Interactable = false;

            if (activeEntry is RaceTeamEntry && !activeEntry.IsAvailable)
            {
                _instructionPanel.onClick.AddListener(GoToTeamCreation);
                _instructionPanel.Interactable = true;
            }
        }

        #endregion
    }
}