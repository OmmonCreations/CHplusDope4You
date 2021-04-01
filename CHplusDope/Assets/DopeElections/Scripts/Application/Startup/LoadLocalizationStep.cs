using Localizator;

namespace DopeElections.Startup
{
    public class LoadLocalizationStep : DopeElectionsStartupStep
    {
        public LoadLocalizationStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            App.ReloadLocalization(OnLocalizationLoaded);
        }

        private void OnLocalizationLoaded(ILocalization localization)
        {
            Complete(localization != null);
        }
    }
}