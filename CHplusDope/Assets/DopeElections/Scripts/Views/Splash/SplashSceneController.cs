using AppManagement;
using BlackMasks;
using DopeElections.ScriptedSequences.Splash;
using Essentials;
using UnityEngine;

namespace DopeElections.Splash
{
    public class SplashSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.Splash;

        [SerializeField] private BlackMask _blackMask = null;
        [SerializeField] private SplashViewsContainer _viewsContainer = null;
        [SerializeField] private SplashCinematicController _cinematicController = null;

        private bool StartupComplete { get; set; }

        private SplashViewsContainer Views => _viewsContainer;
        private SplashCinematicController CinematicController => _cinematicController;
        private BlackMask BlackMask => _blackMask;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize();
            CinematicController.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<SplashSceneLoadParams>();
            StartupComplete = loadParams != null && loadParams.StartupComplete;

            Views.SplashView.Open(OnStartGameConfirmed);

            CinematicController.Play(OnCinematicCompleted, OnCinematicCancelled);
        }

        private void OnCinematicCompleted()
        {
            GameReady();
        }

        private void OnCinematicCancelled()
        {
            GameReady();
        }

        private void GameReady()
        {
        }

        private void OnStartGameConfirmed()
        {
            if (CinematicController.IsPlaying)
            {
                CinematicController.Stop();
            }

            if (!StartupComplete)
            {
                BlackMask.FadeToBlack(() => DopeElectionsApp.Instance.RunStartupProcedure());
                return;
            }

            BlackMask.FadeToBlack(() => ApplicationController.LaunchApp());
        }
    }
}