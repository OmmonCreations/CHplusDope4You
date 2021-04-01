namespace DopeElections.Startup
{
    public class LoadSettingsStep : DopeElectionsStartupStep
    {
        public LoadSettingsStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            var settings = App.Settings;
            var settingsJson = App.LocalStorage.GetJson(settings.File);
            if (settingsJson != null)
            {
                settings.Load(settingsJson);
            }
            else
            {
                settings.SetDefaultValues();
                App.SaveSettings();
            }
            
            settings.ApplyValues();
            Complete(true);
        }
    }
}