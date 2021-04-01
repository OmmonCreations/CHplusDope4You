using System;
using UnityEngine;

namespace AsyncListeners
{
    public class AsyncOneShotListener : AsyncListener
    {
        public delegate void ListenEvent();

        public event ListenEvent OnComplete;
        public event ListenEvent OnTimeout;
        
        private float Timeout { get; }

        private float _t;
        
        public AsyncOneShotListener(Func<bool> listener, float timeout) : base(listener)
        {
            Timeout = timeout;
        }

        internal override void Listen()
        {
            if (Listener.Invoke())
            {
                Complete();
                return;
            }

            if (Timeout >= 0)
            {
                _t += Time.deltaTime;
                if (_t >= Timeout)
                {
                    TriggerTimeout();
                }
            }
        }

        public void Cancel()
        {
            Finish();
        }

        private void Complete()
        {
            Finish();
            if (OnComplete != null) OnComplete();
        }

        private void TriggerTimeout()
        {
            Finish();
            if (OnTimeout != null) OnTimeout();
        }

        private void Finish()
        {
            IsFinished = true;
        }
    }
}