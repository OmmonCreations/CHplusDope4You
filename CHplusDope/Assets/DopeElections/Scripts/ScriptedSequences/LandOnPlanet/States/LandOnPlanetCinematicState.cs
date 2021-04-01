using DopeElections.HotAirBalloon;
using DopeElections.Planets;
using DopeElections.Users;

namespace DopeElections.ScriptedSequences.LandOnPlanet.States
{
    public abstract class LandOnPlanetCinematicState : ScriptedSequenceState<LandOnPlanetCinematicController>
    {
        protected PlayerController Player { get; }
        protected HotAirBalloonController Balloon { get; }
        protected PlanetController Planet { get; }
        
        protected LandOnPlanetCinematicState(LandOnPlanetCinematicController controller) : base(controller)
        {
            Player = controller.Player;
            Balloon = controller.HotAirBalloon;
            Planet = controller.Planet;
        }
    }
}