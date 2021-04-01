using AppManagement;
using DopeElections.Races;

namespace DopeElections.RaceResults
{
    public class RaceResultSceneLoadParams : SceneLoadParams
    {
        public RaceContext Context { get; }
        public bool PlayRaceFinishSequence { get; }

        public RaceResultSceneLoadParams(RaceContext context, bool playRaceFinishSequence)
        {
            Context = context;
            PlayRaceFinishSequence = playRaceFinishSequence;
        }
    }
}