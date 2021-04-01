using AppManagement;
using DopeElections.Sounds;
using Essentials;
using FMODSoundInterface;
using UnityEngine;
using UnityEngine.Serialization;

namespace DopeElections.RaceResults
{
    public class RaceResultSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.RaceResult;
        public override InitializeTrigger Initialization => InitializeTrigger.AfterLaunch;
        
        [SerializeField] private RaceResultViewsContainer _viewsContainer = null;

        private RaceResultViewsContainer Views => _viewsContainer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<RaceResultSceneLoadParams>();
            if (loadParams == null)
            {
                DopeElectionsRouter.GoToProgress();
                return;
            }
            
            MusicController.Play(Music.RaceResult);
            
            if (loadParams.PlayRaceFinishSequence)
            {
                Views.CelebrationView.Open(loadParams.Context);
            }
            else
            {
                Views.ReviewView.Open(loadParams.Context);
            }
        }
    }
}