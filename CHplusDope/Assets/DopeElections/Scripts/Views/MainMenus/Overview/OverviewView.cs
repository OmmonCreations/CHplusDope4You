using System.Linq;
using DopeElections.Accounts;
using DopeElections.Localizations;
using DopeElections.Progression;
using DopeElections.Progression.Questionnaire;
using DopeElections.Users;
using Essentials;
using Localizator;
using MobileInputs;
using Popups;
using Progression;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.MainMenus
{
    public class OverviewView : MainMenuView
    {
        public override NamespacedKey Id => MainMenuViewId.Overview;

        [SerializeField] private ExtraSettingsButton _extraSettingsButton = null;
        [SerializeField] private LocalizedText _raceText = null;
        [SerializeField] private LocalizedText _voteText = null;
        [SerializeField] private FirstRaceTutorialController _firstRaceTutorialController = null;

        [Header("Main Navigation")] [SerializeField]
        private Button3d _listsAndCandidatesButton = null;

        [SerializeField] private Button3d _teamButton = null;
        [SerializeField] private Button3d _editAccountButton = null;
        [SerializeField] private Button3d _playButton = null;
        [SerializeField] private bool _allowListsAndCandidates = true;
        [SerializeField] private bool _allowTeam = true;
        [SerializeField] private bool _allowRace = true;
        [SerializeField] private bool _allowEditAccount = true;

        [Header("Corner Navigation")] [SerializeField]
        private Button _locationSelectionButton = null;

        [SerializeField] private Button _settingsButton = null;
        [SerializeField] private Button _voteButton = null;
        [SerializeField] private Button _helpButton = null;

        private bool _mustDoRace;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _raceText.key = LKey.Views.Overview.Race;

            _voteText.key = LKey.Views.Overview.Vote;

            _editAccountButton.onClick.AddListener(GoToAccount);
            _playButton.onClick.AddListener(GoToProgress);

            _voteButton.onClick.AddListener(GoToVotingInfo);
            _helpButton.onClick.AddListener(GoToInformations);
            _settingsButton.onClick.AddListener(GoToSettings);
            _locationSelectionButton.onClick.AddListener(GoToLocationSelection);

            _firstRaceTutorialController.Initialize();

            _listsAndCandidatesButton.interactable = false;
            _teamButton.interactable = false;
            _editAccountButton.interactable = false;
            _playButton.interactable = false;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _extraSettingsButton.CollapseImmediate();
            Views.Background.Hide();

            var userJourney = DopeElectionsApp.Instance.User.UserJourney;
            if (userJourney.GetEntry(UserJourneyStepId.FirstRace).State != ProgressEntry.ProgressState.Completed)
            {
                _mustDoRace = true;
                _firstRaceTutorialController.Show();
            }
            else
            {
                _mustDoRace = false;
                _firstRaceTutorialController.Hide();
            }

            Views.BlackMask.BlockInteractions(false);
            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();

            _listsAndCandidatesButton.interactable = !_mustDoRace && _allowListsAndCandidates;
            _teamButton.interactable = !_mustDoRace && _allowTeam;
            _editAccountButton.interactable = !_mustDoRace && _allowEditAccount;
            _playButton.interactable = _allowRace;

            PerformQuestionnaireTranslationCheck();
        }

        protected override void OnClose()
        {
            base.OnClose();
            _listsAndCandidatesButton.interactable = false;
            _teamButton.interactable = false;
            _editAccountButton.interactable = false;
            _playButton.interactable = false;
        }

        private void GoToAccount()
        {
            if (!IsOpen || _mustDoRace) return;
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => DopeElectionsRouter.GoToAccount(AccountViewId.FaceSelection));
        }

        private void GoToProgress()
        {
            if (!IsOpen) return;
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(DopeElectionsRouter.GoToProgress);
        }

        private void GoToVotingInfo()
        {
            if (_mustDoRace) return;
            Views.VoteInfosView.Open();
        }

        private void GoToInformations()
        {
            Views.InformationsView.Open();
        }

        private void GoToSettings()
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => DopeElectionsRouter.GoToAccount(AccountViewId.Settings));
        }

        private void GoToLocationSelection()
        {
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(() => DopeElectionsRouter.GoToAccount(AccountViewId.LocationSelection));
        }

        private void PerformQuestionnaireTranslationCheck()
        {
            var app = DopeElectionsApp.Instance;
            var user = app.User;
            var questionnaire = user.Questionnaire;
            if (questionnaire == null) return;
            var tree = questionnaire.Progression;
            var checkerEntryId = RaceProgressStepId.TranslationCheck;
            var checkerEntry = tree.GetEntry<TranslationCheckEntry>(checkerEntryId);
            checkerEntry.PerformTranslationCheck().Then(change =>
            {
                if (change) user.Save();
            });
        }
    }
}