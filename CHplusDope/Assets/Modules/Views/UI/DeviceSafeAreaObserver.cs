using System;
using UnityEngine;

namespace Views.UI
{
    public class DeviceSafeAreaObserver : MonoBehaviour
    {
        public delegate void Event();

        public static event Event SafeAreaChanged = delegate { };
        
        private static DeviceSafeAreaObserver _instance;

        private Rect _safeArea;

        public static Rect SafeArea => _instance._safeArea;

        private void Update()
        {
            var safeArea = Screen.safeArea;
            if (_safeArea == safeArea) return;
            _safeArea = safeArea;
            SafeAreaChanged();
        }

        internal static void EnsureInstancePresent()
        {
            if (_instance) return;
            var gameObject = new GameObject("DeviceSafeAreaObserver");
            _instance = gameObject.AddComponent<DeviceSafeAreaObserver>();
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}