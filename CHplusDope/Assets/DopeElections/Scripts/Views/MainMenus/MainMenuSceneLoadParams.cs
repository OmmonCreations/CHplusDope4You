using AppManagement;
using Essentials;

namespace DopeElections.MainMenus
{
    public class MainMenuSceneLoadParams : SceneLoadParams
    {
        public NamespacedKey ViewId { get; }

        public MainMenuSceneLoadParams(NamespacedKey viewId)
        {
            ViewId = viewId;
        }
    }
}