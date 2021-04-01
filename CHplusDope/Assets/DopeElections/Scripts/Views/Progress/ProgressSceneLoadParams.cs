using AppManagement;
using DopeElections.Progression;
using Essentials;

namespace DopeElections.Progress
{
    public class ProgressSceneLoadParams : SceneLoadParams
    {
        public NamespacedKey ViewId { get; }
        public IRaceProgressEntry FocusedProgressEntry { get; }

        public ProgressSceneLoadParams(NamespacedKey viewId, IRaceProgressEntry focused)
        {
            ViewId = viewId;
            FocusedProgressEntry = focused;
        }
    }
}