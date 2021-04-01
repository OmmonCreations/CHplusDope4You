using System;
using FMODUnity;
using StateMachines;
using UnityEngine;
using UnityEngine.UI;

namespace FMODSoundInterface
{
    public class MusicController : MonoBehaviour
    {
        private static MusicController _instance;

        private StateMachine _mainStateMachine = null;
        private StateMachine _alternativeStateMachine = null;

        private StateMachine MainStateMachine => _mainStateMachine;
        private StateMachine AlternativeStateMachine => _alternativeStateMachine;

        private string _musicId;
        private float _volume;

        private StudioEventEmitter _currentEmitter = null;

        private void Update()
        {
            _mainStateMachine.Run();
            _alternativeStateMachine.Run();
        }

        private void PlayMusic(string musicId, float crossFade)
        {
            if (musicId == _musicId)
            {
                return;
            }

            _musicId = musicId;

            StopPreviousTransitions();

            var emitter = musicId != null ? CreateEmitter(musicId) : null;
            var previousEmitter = _currentEmitter;

            _volume = 1;

            if (crossFade > 0)
            {
                if (previousEmitter)
                {
                    FadeOut(previousEmitter, AlternativeStateMachine, crossFade);
                }

                if (emitter)
                {
                    FadeIn(emitter, MainStateMachine, crossFade);
                }
            }
            else
            {
                if (previousEmitter)
                {
                    previousEmitter.Stop();
                    Destroy(previousEmitter);
                }

                if (emitter) emitter.Play();
            }

            _currentEmitter = emitter;
        }

        private void StopPreviousTransitions()
        {
            MainStateMachine.State = null;
            AlternativeStateMachine.State = null;
        }

        private StudioEventEmitter CreateEmitter(string musicId)
        {
            var result = gameObject.AddComponent<StudioEventEmitter>();
            try
            {
                result.Event = musicId;
            }
            catch
            {
                Debug.LogWarning("Music " + musicId + " not found.");
            }

            return result;
        }

        private static TransitionState FadeOut(StudioEventEmitter emitter, StateMachine stateMachine, float time)
        {
            var transition = Fade(emitter, stateMachine, 1, 0, time);
            transition.OnFinished += () => Destroy(emitter);
            return transition;
        }

        private static TransitionState FadeIn(StudioEventEmitter emitter, StateMachine stateMachine, float time)
        {
            try
            {
                emitter.Play();
            }
            catch
            {
                Debug.LogWarning("Music " + emitter.Event + " not found");
            }

            return Fade(emitter, stateMachine, 0, 1, time);
        }

        private static TransitionState Fade(StudioEventEmitter emitter, StateMachine stateMachine, float from, float to,
            float time)
        {
            var transition = new TransitionState(time, from, to);
            transition.OnTransition += t => emitter.SetParameter("volume", t);
            transition.OnCompleted += () => emitter.SetParameter("volume", to);
            if (to <= 0) transition.OnCompleted += emitter.Stop;
            stateMachine.State = transition;
            return transition;
        }

        private static void CreateInstance()
        {
            var instanceObject = new GameObject("MusicController");
            DontDestroyOnLoad(instanceObject);
            instanceObject.hideFlags = HideFlags.HideAndDontSave;
            var instance = instanceObject.AddComponent<MusicController>();
            instance._mainStateMachine = instanceObject.AddComponent<StateMachine>();
            instance._alternativeStateMachine = instanceObject.AddComponent<StateMachine>();

            _instance = instance;
        }

        public static TransitionState Duck(bool duck, float time = 0.5f)
        {
            if (!_instance || !_instance._currentEmitter) return null;
            var instance = _instance;
            var emitter = instance._currentEmitter;
            var transition = new TransitionState(time, 0, 1);
            var start = instance._volume;
            var target = duck ? 0.5f : 1;
            transition.OnTransition += t =>
            {
                if (!emitter) return;
                var volume = Mathf.Lerp(start, target, t);
                emitter.SetParameter("volume", volume);
                instance._volume = volume;
            };
            transition.OnCompleted += () =>
            {
                if (!emitter) return;
                emitter.SetParameter("volume", target);
                instance._volume = target;
            };
            instance.MainStateMachine.State = transition;
            return transition;
        }

        public static void Play(string musicId, float crossFade = 0.5f)
        {
            if (!_instance) CreateInstance();
            var instance = _instance;
            instance.PlayMusic(musicId, crossFade);
        }
    }
}