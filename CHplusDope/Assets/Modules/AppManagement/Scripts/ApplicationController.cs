using System;
using System.Linq;
using AppSettings;
using Essentials;
using FileStore;
using Procedures;
using TaskScheduling;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AppManagement
{
    public abstract class ApplicationController : MonoBehaviour
    {
        private static ApplicationController _instance;

        private static SceneLoadParams SceneLoadParams { get; set; }

        public TaskScheduler TaskScheduler { get; private set; }

        public LocalStorage LocalStorage { get; private set; }
        public InternalStorage InternalStorage { get; private set; }

        public SettingsContainer Settings { get; private set; }

        protected abstract string AppName { get; }
        protected abstract string VersionString { get; }

        private ScenesContainer _scenesContainer = null;

        public NamespacedKey ActiveSceneId => _scenesContainer.ActiveSceneId;

        protected void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);

            OnAwake();
            Initialize();
        }

        protected virtual void OnAwake()
        {
        }

        protected void Start()
        {
            OnStart();
            RunStartupProcedure();
        }

        protected virtual void OnStart()
        {
        }

        private void Initialize()
        {
            LocalStorage = new LocalStorage();
            LocalStorage.Initialize();
            InternalStorage = new InternalStorage();
            InternalStorage.Initialize();
            TaskScheduler = TaskScheduler.Create();
            Settings = InitializeSettings();
            InitializeScenes();
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        protected abstract SettingsContainer InitializeSettings();

        public void RunStartupProcedure()
        {
            Debug.Log($"Initializing {AppName} on version {VersionString}...");
            var procedure = new Procedure<bool>(GetStartupSteps());
            procedure.Run(true, CompleteStartupProcedure);
        }

        protected void InitializeScenes()
        {
            _scenesContainer = FindObjectOfType<ScenesContainer>();
            _scenesContainer.SceneLoaded += OnSceneChanged;
            _scenesContainer.Initialize();
            _scenesContainer.Initialize(SceneController.InitializeTrigger.BeforeStartup);
        }

        public static void LaunchApp(Action<bool> callback = null)
        {
            _instance.RunLaunchProcedure(callback);
        }

        protected abstract ProcedureStep<bool>[] GetStartupSteps();

        protected virtual void CompleteStartupProcedure(bool success)
        {
            if (!success)
            {
                Debug.LogError("Startup failed!");
                return;
            }

            Debug.Log($"Done! {AppName} version {VersionString}");
        }

        public void RunLaunchProcedure(Action<bool> callback = null)
        {
            _scenesContainer.Initialize(SceneController.InitializeTrigger.BeforeLaunch);
            var procedure = new Procedure<bool>(GetLaunchSteps());
            procedure.Run(true, success =>
            {
                CompleteLaunchProcedure(success);
                if (callback != null) callback(success);
            });
        }

        protected abstract ProcedureStep<bool>[] GetLaunchSteps();

        protected virtual void CompleteLaunchProcedure(bool success)
        {
            _scenesContainer.Initialize(SceneController.InitializeTrigger.AfterLaunch);
        }

        protected void TriggerSceneChanged()
        {
            OnSceneChanged(_scenesContainer.ActiveSceneId);
        }

        private void OnSceneChanged(NamespacedKey sceneId)
        {
            if (SceneLoadParams is PostSceneLoadAction loadAction)
            {
                // Debug.Log("Execute PostSceneLoadAction.");
                loadAction.Callback();
            }
        }

        public static void LoadScene(NamespacedKey sceneId, SceneLoadParams loadParams = null)
        {
            SceneLoadParams = loadParams;

            _instance._scenesContainer.Load(sceneId, loadParams);
        }
    }
}