using System;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FMODSoundInterface
{
    public static class SoundController
    {
        public static string[] PausableCategories { get; set; }

        public static void Play(string sound, Vector3 position)
        {
            if (string.IsNullOrWhiteSpace(sound)) return;
            try
            {
                RuntimeManager.PlayOneShot(sound, position);
            }
            catch
            {
                Debug.LogWarning("Sound " + sound + " not found.");
            }
        }

        public static void Play(string sound, GameObject gameObject)
        {
            if (string.IsNullOrWhiteSpace(sound)) return;
            try
            {
                RuntimeManager.PlayOneShotAttached(sound, gameObject);
            }
            catch
            {
                Debug.LogWarning("Sound " + sound + " not found.");
            }
        }

        public static void Play(string sound)
        {
            Play(sound, Vector3.zero);
        }

        public static void SetVolume(string category, float volume)
        {
            try
            {
                var bus = RuntimeManager.GetBus(category);
                bus.setVolume(volume);
                bus.setMute(volume <= 0);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                //Debug.LogWarning("Bus "+category+" not found");
            }
        }

        public static void Pause()
        {
            foreach (var category in PausableCategories)
            {
                try
                {
                    var bus = RuntimeManager.GetBus(category);
                    bus.setPaused(true);
                }
                catch
                {
                    Debug.LogWarning("Bus " + category + " not found");
                }
            }
        }

        public static void Resume()
        {
            foreach (var category in PausableCategories)
            {
                try
                {
                    var bus = RuntimeManager.GetBus(category);
                    bus.setPaused(false);
                }
                catch
                {
                    Debug.LogWarning("Bus " + category + " not found");
                }
            }
        }
    }
}