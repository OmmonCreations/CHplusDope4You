using UnityEngine;
using UnityEngine.SceneManagement;

namespace DopeElections.Startup
{
    public class ReloadGameStep : DopeElectionsStartupStep
    {
        public ReloadGameStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(0);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
        {
            Resources.UnloadUnusedAssets().completed += operation => { Complete(true); };
        }
    }
}