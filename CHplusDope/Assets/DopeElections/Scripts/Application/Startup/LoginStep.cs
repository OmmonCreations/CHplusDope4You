using DopeElections.Progression;
using Progression;

namespace DopeElections.Startup
{
    public class LoginStep : DopeElectionsStartupStep
    {
        public LoginStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }
            // Debug.Log("go trough login step");
            var user = DopeElectionsApp.Instance.User;
            user.UserJourney = UserJourneyTree.Get();
            user.Load();

            var characterCreationProgress = user.UserJourney.GetEntry(UserJourneyStepId.CharacterCreation);
            if (characterCreationProgress.State == ProgressEntry.ProgressState.Completed)
            {
                // Debug.Log("Logged in automatically!");
                OnLoginComplete(true);
                return;
            }

            // Debug.Log("Go to login screen...");
            DopeElectionsRouter.GoToAccountCreation(success =>
            {
                if (!success)
                {
                    OnLoginComplete(false);
                    return;
                }

                characterCreationProgress.State = ProgressEntry.ProgressState.Completed;
                user.Save();
                
                OnLoginComplete(true);
            });
        }

        private void OnLoginComplete(bool success)
        {
            // Debug.Log("Login complete!");
            base.Complete(success);
        }
    }
}