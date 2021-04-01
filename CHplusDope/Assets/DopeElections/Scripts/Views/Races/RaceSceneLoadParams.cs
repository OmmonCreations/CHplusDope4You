using AppManagement;

namespace DopeElections.Races
{
    public class RaceSceneLoadParams : SceneLoadParams
    {
        public RaceContext Context { get; }

        public RaceSceneLoadParams(RaceContext context)
        {
            Context = context;
        }
    }
}