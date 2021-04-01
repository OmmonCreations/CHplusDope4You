namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class IdleState : GrabArmState
    {
        public IdleState(GrabArmController controller) : base(controller)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Controller.PlayIdleAnimation();
        }

        public override void Update()
        {
        }
    }
}