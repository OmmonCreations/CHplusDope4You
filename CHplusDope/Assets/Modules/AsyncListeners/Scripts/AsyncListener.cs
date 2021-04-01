using System;

namespace AsyncListeners
{
    public abstract class AsyncListener
    {
        protected Func<bool> Listener { get; }
        
        public bool IsFinished { get; protected set; }

        protected AsyncListener(Func<bool> listener)
        {
            Listener = listener;
        }

        internal abstract void Listen();
    }
}