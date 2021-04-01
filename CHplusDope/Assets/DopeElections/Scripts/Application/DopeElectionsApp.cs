using System;
using AppManagement;
using AppSettings;
using DopeElections.Splash;
using DopeElections.Startup;
using DopeElections.Users;
using Localizator;
using Notifications;
using Popups;
using Procedures;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections
{
    public class DopeElectionsApp : ApplicationController
    {
        public delegate void UserEvent(ActiveUser user);

        public event UserEvent UserChanged = delegate { };

        public delegate void LocalizationEvent(ILocalization localization);

        public event LocalizationEvent LocalizationChanged = delegate { };

        public static DopeElectionsApp Instance { get; private set; }

        public const string Namespace = "dope_elections";

        protected override string AppName => Application.productName;
        protected override string VersionString => Application.version;

        [SerializeField] private AssetsLibrary[] _builtinAssets = null;
        [SerializeField] private GameNotificationsManager _notificationsManager = null;
        [SerializeField] private GenericViewsContainer _genericViews = null;
        [SerializeField] private PopupsLayer _popupsLayer = null;

        private ActiveUser _user = new ActiveUser();

        public AssetPack Assets { get; internal set; }
        public GameNotificationsManager Notifications => _notificationsManager;
        public BackendCHplusDope RestApi { get; private set; }

        public ActiveUser User
        {
            get => _user;
            private set
            {
                _user = value;
                UserChanged(value);
            }
        }

        public GenericViewsContainer Views => _genericViews;
        public PopupsLayer Popups => _popupsLayer;
        public ILocalization Localization { get; private set; } = new DefaultLocalization();
        internal AssetsLibrary[] BuiltinAssets => _builtinAssets;

        protected override void OnAwake()
        {
            RestApi = new BackendCHplusDope();

            base.OnAwake();
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Instance = this;
#if UNITY_EDITOR
            Application.targetFrameRate = 120;
#else
            Application.targetFrameRate = 60;
#endif
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Views.Initialize();
            Notifications.Initialize();
        }

        protected override SettingsContainer InitializeSettings()
        {
            return new SettingsContainer("settings.json",
                Setting.EulaAccepted,
                Setting.DeviceId,
                Setting.Language,
                Setting.MasterVolume,
                Setting.SfxVolume,
                Setting.UIVolume,
                Setting.MusicVolume,
                Setting.AmbienceVolume,
                Setting.EnableRaceMusic
            );
        }

        protected override ProcedureStep<bool>[] GetStartupSteps()
        {
            return new ProcedureStep<bool>[]
            {
                new LoadSettingsStep(this),
                new GoToStartupSceneStep(this),
                new LoadLocalizationStep(this),
                new FadeToClearStep<StartupSceneController>(this),
                new PresentEulaStep(this),
                new PreloadCoreAssetsStep(this, 0, 0.2f),
                new DownloadLoggedStep(this, 0.2f, 0.8f),
                new LoadAssetsStep(this, 0.8f, 1f),
                new FadeToBlackStep<StartupSceneController>(this),
                new LoadSceneStep(this, SceneId.Splash, new SplashSceneLoadParams(true)),
            };
        }

        protected override ProcedureStep<bool>[] GetLaunchSteps()
        {
            return new ProcedureStep<bool>[]
            {
                new LoginStep(this),
                new LoadSceneStep(this, SceneId.MainMenu),
            };
        }

        protected override void CompleteStartupProcedure(bool success)
        {
            if (!success)
            {
                DopeElectionsRouter.GoToSplash(false);
                return;
            }

            Setting.Language.LanguageChanged += code => ReloadLocalization();

            base.CompleteStartupProcedure(true);
        }

        protected override void CompleteLaunchProcedure(bool success)
        {
            if (!success)
            {
                DopeElectionsRouter.GoToSplash();
                return;
            }

            base.CompleteLaunchProcedure(true);
        }

        public void SaveSettings()
        {
            LocalStorage.WriteAllText(Settings.File, Settings.Save().ToString());
        }

        public void DeleteDownloadedData()
        {
            LocalStorage.DeleteFiles(BackendCHplusDope.DownloadsDirectory);
        }

        public void DeleteAllData()
        {
            LocalStorage.DeleteFiles();
            Restart();
        }

        public void Restart()
        {
            User = new ActiveUser();
            Assets.Clear();
            var procedure = new Procedure<bool>(
                new ReloadGameStep(this)
            );
            procedure.Run(true, success =>
            {
                InitializeScenes();
                RunStartupProcedure();
            });
        }

        public void ReloadAssets()
        {
            Assets.Clear();
            var procedure = new Procedure<bool>(
                new GoToStartupSceneStep(this),
                new FadeToClearStep<StartupSceneController>(this),
                new PreloadCoreAssetsStep(this, 0, 0.2f),
                new DownloadLoggedStep(this, 0.2f, 0.8f),
                new LoadAssetsStep(this, 0.8f, 1f),
                new ReloadUserStep(this),
                new FadeToBlackStep<StartupSceneController>(this)
            );
            procedure.Run(true, success =>
            {
                if (success) DopeElectionsRouter.GoToMainMenu();
                else Restart();
            });
        }

        public void ReloadLocalization(Action<ILocalization> callback = null)
        {
            var code = Settings.GetValue(Setting.Language);
            LoadLocalization(code, localization =>
            {
                Localization = localization;
                LocalizationChanged(localization);
                if (callback != null) callback(localization);
            });
        }

        private void LoadLocalization(string language, Action<ILocalization> callback)
        {
            if (language == null)
            {
                Debug.LogWarning("Language code is null.");
                callback(new DefaultLocalization());
                return;
            }

            var internalStorage = InternalStorage;
            internalStorage.ReadAllText($"lang/{language}.json", data =>
            {
                var localization = Localizator.Localization.Deserialize(data);
                callback(localization);
            });
        }
    }
}