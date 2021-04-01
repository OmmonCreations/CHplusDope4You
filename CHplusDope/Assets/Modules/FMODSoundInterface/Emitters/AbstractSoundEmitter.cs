using System;
using UnityEngine;

namespace FMODSoundInterface
{
    public abstract class AbstractSoundEmitter : MonoBehaviour
    {
        protected void OnEnable()
        {
            HookEvents();
        }

        protected void OnDisable()
        {
            ReleaseHooks();
        }

        public abstract void HookEvents();
        public abstract void ReleaseHooks();
    }

    public abstract class AbstractSoundEmitter<T> : AbstractSoundEmitter
    {
        protected abstract T Source { get; }

        private T m_source;

        public sealed override void HookEvents()
        {
            var source = Source;
            if (source != null) HookEvents(source);
            m_source = source;
        }

        public sealed override void ReleaseHooks()
        {
            if (m_source == null) return;
            ReleaseHooks(m_source);
        }

        public abstract void HookEvents(T source);
        public abstract void ReleaseHooks(T source);
    }
}