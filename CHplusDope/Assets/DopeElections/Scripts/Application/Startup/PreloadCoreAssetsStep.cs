using DopeElections.Localizations;
using FileStore;
using Popups;
using RuntimeAssetImporter;

namespace DopeElections.Startup
{
    public class PreloadCoreAssetsStep : ProgressDisplayingStep
    {
        private BackendCHplusDope RestApi { get; set; }
        private LocalStorage Storage { get; }
        private string LanguageCode { get; set; }

        private bool _error;

        public PreloadCoreAssetsStep(DopeElectionsApp app, float progressStart, float progressEnd) : base(app,
            progressStart, progressEnd)
        {
            Storage = app.LocalStorage;
        }

        public override void Run(bool data)
        {
            base.Run(data);
            if (!data)
            {
                Complete(false);
                return;
            }

            RestApi = App.RestApi;
            LanguageCode = BackendCHplusDope.GetLanguageCode(App.Settings.GetValue(Setting.Language));
            RestApi.Language = LanguageCode;

            Progress = 0;
            ProgressLabel = LKey.Views.Startup.Connecting;
            App.Assets = new AssetPack();
            DownloadCantons();
        }

        public void DownloadCantons()
        {
            var file = BackendCHplusDope.GetCantonsFile();

            RestApi.GetCantons(data =>
            {
                if (data == null && !IsPresent(file))
                {
                    CallError();
                    return;
                }

                if (data != null) Storage.WriteAllText(file, data, true);
                DownloadConstituencies();
            });
        }

        public void DownloadConstituencies()
        {
            Progress = 0.5f;
            var file = BackendCHplusDope.GetConstituenciesFile(LanguageCode);
            RestApi.GetConstituencies(data =>
            {
                if (data == null && !IsPresent(file))
                {
                    CallError();
                    return;
                }

                if (data != null) Storage.WriteAllText(file, data, true);
                DownloadConstituencyDone();
            });
        }

        private void DownloadConstituencyDone()
        {
            // Debug.Log("Download complete!");
            Progress = 1;
            if (_error) return;
            Complete(true);
        }

        private bool IsPresent(string file)
        {
            return App.LocalStorage.FileExists(file);
        }

        private void CallError()
        {
            if (_error) return;
            _error = true;
            App.Popups.ShowPopup(new AlertPopup(
                LKey.Views.Startup.DownloadFailedAlert.Title,
                LKey.Views.Startup.DownloadFailedAlert.Text
            ).Then(() => Complete(false)));
        }
    }
}