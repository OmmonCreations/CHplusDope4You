using AppManagement;
using DopeElections.Parliaments;
using DopeElections.Progression;
using DopeElections.Sounds;
using DopeElections.Users;
using Essentials;
using FMODSoundInterface;
using Progression;
using UnityEngine;
using UnityEngine.Serialization;
using Views;

namespace DopeElections.MainMenus
{
    public class MainMenuSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.MainMenu;
        public override InitializeTrigger Initialization => InitializeTrigger.BeforeLaunch;

        [FormerlySerializedAs("_menuController")] [SerializeField]
        private MainMenuViewsContainer _viewsContainer = null;

        [SerializeField] private ParliamentController _parliament = null;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private GameObject _overviewEnvironment = null;

        private MainMenuViewsContainer Views => _viewsContainer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _viewsContainer.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<MainMenuSceneLoadParams>();
            
            _playerController.ApplyUserConfiguration();
            _playerController.PlayIdleAnimation();

            var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
            var gameCompleted = questionnaire != null && questionnaire.Progression
                .GetEntry(RaceProgressStepId.EndingComic)
                .State == ProgressEntry.ProgressState.Completed;
            _parliament.gameObject.SetActive(gameCompleted);
            _parliament.Locked = !gameCompleted;

            var viewId = GetViewId(loadParams);
            
            _overviewEnvironment.SetActive(viewId != MainMenuViewId.EndingCinematic);

            Views.BlackMask.Alpha = 1;
            MusicController.Play(Music.MainMenu);

            var view = Views.GetView<IView>(viewId);
            view.Open();
            
        }

        private NamespacedKey GetViewId(MainMenuSceneLoadParams loadParams)
        {
            if (DopeElectionsApp.Instance.User.UserJourney.GetEntry(UserJourneyStepId.MainMenuLanding).State !=
                ProgressEntry.ProgressState.Completed)
            {
                return Views.LandSequenceView.Id;
            }
            
            return loadParams != null && loadParams.ViewId != default
                ? loadParams.ViewId
                : MainMenuViewId.Overview;
        }
    }
}