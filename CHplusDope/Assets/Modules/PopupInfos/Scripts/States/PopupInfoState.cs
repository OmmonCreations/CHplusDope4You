using StateMachines;

namespace PopupInfos
{
    public abstract class PopupInfoState : State
    {
        protected PopupInfoController Controller { get; }
        
        public PopupInfoState(PopupInfoController controller)
        {
            Controller = controller;
        }
    }
}