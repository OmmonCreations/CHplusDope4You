namespace DopeElections.Startup
{
    public class ReloadUserStep : DopeElectionsStartupStep
    {
        public ReloadUserStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }
            App.User.ReloadQuestionnaire();
            Complete(true);
        }
    }
}