using StateMachines;

namespace DopeElections.Races
{
    public abstract class CandidateGroupState : State
    {
        protected RaceController RaceController => GroupController.RaceController;
        protected CandidateGroupController GroupController { get; }
        protected CandidateGroup Group => GroupController.Group;
        
        protected CandidateGroupState(CandidateGroupController controller)
        {
            GroupController = controller;
        }
    }
}