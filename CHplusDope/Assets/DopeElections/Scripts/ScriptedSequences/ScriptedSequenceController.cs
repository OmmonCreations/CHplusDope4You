using System;
using BlackMasks;
using MobileInputs;
using StateMachines;
using UnityEngine;
using UnityEngine.Events;

namespace DopeElections.ScriptedSequences
{
    public abstract class ScriptedSequenceController : MonoBehaviour
    {
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private BlackMask _blackMask = null;
        [SerializeField] private InteractionSystem _interactionSystem = null;
        [SerializeField] private UnityEvent _onInitialize = null;
        [SerializeField] private UnityEvent _onPlay = null;

        protected StateMachine StateMachine => _stateMachine;

        public InteractionSystem InteractionSystem => _interactionSystem;
        public BlackMask BlackMask => _blackMask;

        public bool IsPlaying { get; private set; }
        protected bool IsStopped { get; private set; }

        public void Initialize()
        {
            _onInitialize.Invoke();
            OnInitialize();
        }

        protected void Update()
        {
            StateMachine.Run();
        }

        public void Play(Action complete, Action cancel = null)
        {
            IsStopped = false;
            IsPlaying = true;
            var parts = GetParts();
            if (parts.Length == 0)
            {
                IsPlaying = false;
                complete();
                OnCompleted();
                OnFinished();
                return;
            }

            for (var i = 0; i < parts.Length - 1; i++)
            {
                var index = i;
                var part = parts[i];
                part.OnCompleted += () =>
                {
                    if (!IsPlaying) return;
                    StateMachine.State = parts[index + 1];
                };
                if (part.SkipRange == SkipRange.Everything)
                {
                    part.Skipped += () =>
                    {
                        if (!IsPlaying) return;
                        IsPlaying = false;
                        complete();
                        OnCompleted();
                        OnFinished();
                    };
                }
                part.OnCancelled += () =>
                {
                    if (!IsPlaying) return;
                    IsPlaying = false;
                    if (cancel != null) cancel();
                    OnCancelled();
                    OnFinished();
                };
            }

            parts[parts.Length - 1].OnCompleted += () =>
            {
                if (!IsPlaying) return;
                IsPlaying = false;
                complete();
                OnCompleted();
                OnFinished();
            };
            parts[parts.Length - 1].OnCancelled += () =>
            {
                IsPlaying = false;
                if (cancel != null) cancel();
                OnCancelled();
                OnFinished();
            };

            OnBeforePlay(parts);
            StateMachine.State = parts[0];
            _onPlay.Invoke();
            OnPlay(parts);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnBeforePlay(ScriptedSequenceState[] parts)
        {
        }

        protected virtual void OnPlay(ScriptedSequenceState[] parts)
        {
        }

        protected virtual void OnCompleted()
        {
            
        }

        protected virtual void OnCancelled()
        {
            
        }

        protected virtual void OnFinished()
        {
            
        }
        
        public void Stop()
        {
            if (IsStopped) return;
            IsStopped = true;
            StateMachine.State = null;
            OnStop();
        }

        protected virtual void OnStop()
        {
        }

        protected abstract ScriptedSequenceState[] GetParts();
    }
}