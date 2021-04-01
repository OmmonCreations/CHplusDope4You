using AppManagement;
using DopeElections.Races;

namespace DopeElections.RaceCategorySelections
{
    public class RaceCategorySelectionSceneLoadParams : SceneLoadParams
    {
        public RaceContext Context { get; }

        public RaceCategorySelectionSceneLoadParams(RaceContext context)
        {
            Context = context;
        }
    }
}