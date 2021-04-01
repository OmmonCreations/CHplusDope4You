using AppManagement;
using Essentials;

namespace DopeElections.Startup
{
    public class LoadSceneStep : DopeElectionsStartupStep
    {
        private NamespacedKey SceneId { get; }
        private SceneLoadParams SceneParams { get; }
        
        public LoadSceneStep(DopeElectionsApp app, NamespacedKey sceneId, SceneLoadParams sceneParams = null) : base(app)
        {
            SceneId = sceneId;
            SceneParams = sceneParams;
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }
            ApplicationController.LoadScene(SceneId, SceneParams);
            Complete(true);
        }
    }
}