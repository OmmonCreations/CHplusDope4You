using StateMachines;

namespace DopeElections.MainMenus
{
    public abstract class EndingCreditsState : State
    {
        protected EndingCreditsView View { get; }

        protected EndingCreditsState(EndingCreditsView view)
        {
            View = view;
        }
    }
}