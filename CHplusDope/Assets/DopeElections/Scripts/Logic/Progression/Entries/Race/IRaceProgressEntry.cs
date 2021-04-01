using DopeElections.Races;
using Localizator;
using Progression;

namespace DopeElections.Progression
{
    public interface IRaceProgressEntry : IVisibleProgressEntry
    {
        IRace CreateRace(RaceContext context);
    }
}