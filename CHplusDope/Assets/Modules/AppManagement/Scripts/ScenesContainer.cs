using System.Collections.Generic;
using System.Linq;
using Essentials;
using UnityEngine;

namespace AppManagement
{
    public class ScenesContainer : MonoBehaviour
    {
        public delegate void SceneLoadEvent(NamespacedKey sceneId);

        public event SceneLoadEvent SceneLoaded = delegate { };
        public event SceneLoadEvent SceneUnloaded = delegate { };

        [SerializeField] private SceneController[] _scenes = null;

        private readonly List<SceneController.InitializeTrigger> _initializeTriggered =
            new List<SceneController.InitializeTrigger>();

        public NamespacedKey ActiveSceneId => _scenes
            .Where(s => s.gameObject.activeSelf).Select(s => s.Id)
            .FirstOrDefault();

        internal void Initialize()
        {
            foreach (var sceneController in _scenes)
            {
                sceneController.gameObject.SetActive(false);
            }
        }

        internal void Initialize(SceneController.InitializeTrigger trigger)
        {
            if (_initializeTriggered.Contains(trigger)) return;
            _initializeTriggered.Add(trigger);
            foreach (var s in _scenes.Where(s => s.Initialization == trigger)) s.Initialize();
        }

        public void Load(NamespacedKey sceneId, SceneLoadParams loadParams = null)
        {
            var sceneController = _scenes.FirstOrDefault(s => s.Id == sceneId);
            if (sceneController == null)
            {
                Debug.LogError("Scene " + sceneId + " not found!");
                return;
            }

            if (sceneController.gameObject.activeSelf)
            {
                Debug.LogWarning("Scene " + sceneId + " is already loaded.");
                return;
            }

            foreach (var current in _scenes.Where(s => s.gameObject.activeSelf))
            {
                current.Unload();
                SceneUnloaded(current.Id);
            }

            sceneController.Load(loadParams);
            SceneLoaded(sceneId);
        }
    }
}