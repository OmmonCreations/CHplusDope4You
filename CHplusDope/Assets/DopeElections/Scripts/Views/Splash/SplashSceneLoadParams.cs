using AppManagement;

namespace DopeElections.Splash
{
    public class SplashSceneLoadParams : SceneLoadParams
    {
        public bool StartupComplete { get; }

        public SplashSceneLoadParams(bool startupComplete)
        {
            StartupComplete = startupComplete;
        }
    }
}