using Essentials;
using UnityEngine;

namespace AppManagement
{
    public abstract class SceneController : MonoBehaviour
    {
        public abstract NamespacedKey Id { get; }
        public virtual InitializeTrigger Initialization { get; } = InitializeTrigger.BeforeStartup;

        private SceneLoadParams SceneLoadParams { get; set; }
        internal bool Initialized { get; private set; } = false;

        internal void Initialize()
        {
            if (Initialized) return;
            Initialized = true;
            OnInitialize();
        }
        
        public void Load(SceneLoadParams @params)
        {
            SceneLoadParams = @params;
            gameObject.SetActive(true);
            OnLoad();
        }

        internal void Unload()
        {
            gameObject.SetActive(false);
            OnUnload();
        }

        protected virtual void OnInitialize()
        {
            
        }

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnUnload()
        {
            
        }

        protected T GetSceneLoadParams<T>() where T : SceneLoadParams
        {
            return SceneLoadParams as T;
        }

        public enum InitializeTrigger
        {
            BeforeStartup,
            BeforeLaunch,
            AfterLaunch
        }
    }
}