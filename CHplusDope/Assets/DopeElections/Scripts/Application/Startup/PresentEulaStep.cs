using UnityEngine;

namespace DopeElections.Startup
{
    public class PresentEulaStep : DopeElectionsStartupStep
    {
        private StartupSceneController SceneController { get; set; }
        
        public PresentEulaStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }
            SceneController = Object.FindObjectOfType<StartupSceneController>();
            
            if (!App.Settings.GetValue(Setting.EulaAccepted))
            {
                SceneController.Views.Eula.Open(EulaCallback);
                return;
            }
            
            Complete(true);
        }

        private void EulaCallback(bool accepted)
        {
            SceneController.Views.Eula.Close();
            if (!accepted)
            {
                SceneController.Views.BlackMask.FadeToBlack(() =>
                {
                    Complete(false);
                });
                return;
            }

            App.Settings.SetValue(Setting.EulaAccepted, true);
            App.SaveSettings();
            
            Complete(true);
        }
    }
}