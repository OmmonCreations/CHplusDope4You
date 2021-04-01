using AppManagement;
using Essentials;
using UnityEngine;
using UnityEngine.Serialization;

namespace DopeElections.Races
{
    public class RaceSceneController : SceneController
    {
        public override NamespacedKey Id => SceneId.Race;
        public override InitializeTrigger Initialization => InitializeTrigger.AfterLaunch;
        
        [SerializeField] private RaceViewsContainer _viewsContainer = null;
        [SerializeField] private RaceController _raceController = null;

        private RaceController RaceController => _raceController;
        private RaceViewsContainer Views => _viewsContainer;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize(RaceController);
            RaceController.Initialize();
        }

        protected override void OnLoad()
        {
            var loadParams = GetSceneLoadParams<RaceSceneLoadParams>();
            if (loadParams == null)
            {
                Debug.LogError("No load params provided.");
                DopeElectionsRouter.GoToProgress();
                return;
            }

            var context = loadParams.Context;
            if (context.Race == null && !context.CreateRace())
            {
                Debug.LogError("Couldn't create a race.");
                DopeElectionsRouter.GoToProgress();
                return;
            }

            Views.BlackMask.BlockInteractions(false);
            Views.BlackMask.FadeToClear();
            
            RaceController.PrepareRace(context);
            RaceController.StartRace();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            RaceController.Unload();
        }
    }
}