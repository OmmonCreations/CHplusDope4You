using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AsyncListeners
{
    public class AsyncOperations : MonoBehaviour
    {
        private static AsyncOperations _instance = null;

        private readonly List<AsyncListener> _listeners = new List<AsyncListener>();

        private IReadOnlyCollection<AsyncListener> Listeners => _listeners;
        
        private void Update()
        {
            foreach (var listener in Listeners)
            {
                listener.Listen();
            }

            _listeners.RemoveAll(l => l.IsFinished);
        }

        public static AsyncOneShotListener Await(Func<bool> listener, float timeout = -1)
        {
            if (!_instance)
            {
                CreateInstance();
            }
            
            var result = new AsyncOneShotListener(listener, timeout);
            _instance._listeners.Add(result);
            return result;
        }

        public static AsyncRepeatingListener AwaitRepeating(Func<bool> listener)
        {
            if (!_instance)
            {
                CreateInstance();
            }
            var result = new AsyncRepeatingListener(listener);
            _instance._listeners.Add(result);
            return result;
        }

        private static void CreateInstance()
        {
            var instanceObject = new GameObject("NetworkController");
            var instance = instanceObject.AddComponent<AsyncOperations>();
            DontDestroyOnLoad(instance);
            _instance = instance;
        }
    }
}