using DopeElections.HotAirBalloon;
using DopeElections.Planets;
using DopeElections.Users;

namespace DopeElections.ScriptedSequences.JourneyToPlanet
{
    public abstract class JourneyToPlanetCinematicState : ScriptedSequenceState<JourneyToPlanetCinematicController>
    {
        protected PlayerController Player { get; }
        protected HotAirBalloonController Balloon { get; }
        protected PlanetController Planet { get; }
        
        protected JourneyToPlanetCinematicState(JourneyToPlanetCinematicController controller) : base(controller)
        {
            Player = controller.Player;
            Balloon = controller.HotAirBalloon;
            Planet = controller.Planet;
        }
    }
}