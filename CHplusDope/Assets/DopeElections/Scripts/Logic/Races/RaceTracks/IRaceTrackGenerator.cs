using Essentials;

namespace DopeElections.Races.RaceTracks
{
    public interface IRaceTrackGenerator
    {
        void CreateParts(float position);
        void CreateParts();
        RaceTrackPartInstance CreatePart();
        void UnloadParts();
        void UpdateTrackWidth();
    }
}