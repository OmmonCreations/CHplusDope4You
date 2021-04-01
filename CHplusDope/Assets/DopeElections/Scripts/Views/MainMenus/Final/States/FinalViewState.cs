using StateMachines;

namespace DopeElections.MainMenus.Final
{
    public abstract class FinalViewState : State
    {
        protected FinalView View { get; }

        protected FinalViewState(FinalView view)
        {
            View = view;
        }
    }
}