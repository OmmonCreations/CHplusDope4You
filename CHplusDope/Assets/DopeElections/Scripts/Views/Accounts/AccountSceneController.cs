using System;
using AppManagement;
using DopeElections.Parliaments;
using DopeElections.Progression;
using DopeElections.ScriptedSequences.GrabSequence;
using DopeElections.ScriptedSequences.JourneyToPlanet;
using DopeElections.ScriptedSequences.LandOnPlanet;
using DopeElections.Sounds;
using DopeElections.Users;
using Essentials;
using FMODSoundInterface;
using Progression;
using UnityEngine;
using UnityEngine.Serialization;

namespace DopeElections.Accounts
{
    public class AccountSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.Account;
        public override InitializeTrigger Initialization => InitializeTrigger.BeforeLaunch;

        [FormerlySerializedAs("_viewsController")] [FormerlySerializedAs("_accountMenuController")] [SerializeField]
        private AccountViewsContainer _viewsContainer = null;

        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private ParliamentController _parliamentController = null;

        [Header("Scripted Sequences")] [SerializeField]
        private GrabSequenceController _grabSequenceController = null;

        [SerializeField] private JourneyToPlanetCinematicController _journeyToPlanetController = null;

        public ActiveUser User { get; private set; }
        public AccountViewsContainer Views => _viewsContainer;
        public PlayerController PlayerController => _playerController;
        private Action<bool> Callback { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize(this);
            _grabSequenceController.Initialize();
            _journeyToPlanetController.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<AccountSceneLoadParams>() ?? new AccountSceneLoadParams();
            var user = loadParams.User;
            var menuId = loadParams.MenuId;
            var callback = loadParams.Callback;
            Initialize(user, menuId, callback);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Views.LocationSelectionView.Unload();
        }

        private void Initialize(ActiveUser user, NamespacedKey viewId, Action<bool> callback)
        {
            User = user;
            Callback = callback;

            _parliamentController.Locked = user.Questionnaire == null ||
                                           user.Questionnaire.Progression.GetEntry(RaceProgressStepId.EndingComic)
                                               .State != ProgressEntry.ProgressState.Completed;

            _playerController.ApplyUserConfiguration();

            MusicController.Play(Music.Account);

            var grabSequenceProgress = user.UserJourney.GetEntry(UserJourneyStepId.GrabSequence);
            if (grabSequenceProgress.State != ProgressEntry.ProgressState.Completed)
            {
                Views.BlackMask.FadeToClear(2);

                _grabSequenceController.Launch(() =>
                {
                    grabSequenceProgress.State = ProgressEntry.ProgressState.Completed;
                    user.Save();
                    Views.FaceSelectionView.Open(user);
                });
                return;
            }

            if (viewId != default)
            {
                var view = Views.GetView<AccountView>(viewId);
                view.Open(user);
                return;
            }

            if (user.FaceId == default)
            {
                Views.FaceSelectionView.Open(user);
                return;
            }

            Views.LocationSelectionView.Open(user);
        }

        public void PlayJourneyToPlanetCinematic(Action complete)
        {
            Views.CloseViews(Views.LocationSelectionView);
            _journeyToPlanetController.Play(complete, () => DopeElectionsRouter.GoToSplash());
        }

        public void Quit()
        {
            if (Views.BlackMask.Alpha > 0)
            {
                QuitImmediate();
                return;
            }
            Views.BlackMask.BlockInteractions(true);
            Views.BlackMask.FadeToBlack(QuitImmediate);
        }

        public void QuitImmediate()
        {
            var user = User;
            var done = user.CantonId > 0 && user.ConstituencyId > 0;

            if (Callback != null)
            {
                Callback(done);
                return;
            }

            if (done) DopeElectionsRouter.GoToMainMenu();
            else DopeElectionsRouter.GoToSplash();
        }
    }
}