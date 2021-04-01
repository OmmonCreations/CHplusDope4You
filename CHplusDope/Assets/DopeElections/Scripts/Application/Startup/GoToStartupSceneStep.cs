using AppManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DopeElections.Startup
{
    public class GoToStartupSceneStep : DopeElectionsStartupStep
    {
        public GoToStartupSceneStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }

            if (SceneId.Startup == DopeElectionsApp.Instance.ActiveSceneId)
            {
                OnStartupSceneLoaded();
            }
            else
            {
                DopeElectionsRouter.GoToStartup(OnStartupSceneLoaded);
            }
        }

        private void OnStartupSceneLoaded()
        {
            var sceneController = Object.FindObjectOfType<StartupSceneController>();
            sceneController.InitializeViews();
            Complete(true);
        }
    }
}