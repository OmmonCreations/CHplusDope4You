using StateMachines;

namespace DopeElections.PoliticalCharacters
{
    public abstract class PoliticalCharacterState : State
    {
        protected PoliticalCharacterController Controller { get; }
        
        protected PoliticalCharacterState(PoliticalCharacterController character)
        {
            Controller = character;
        }
    }
}