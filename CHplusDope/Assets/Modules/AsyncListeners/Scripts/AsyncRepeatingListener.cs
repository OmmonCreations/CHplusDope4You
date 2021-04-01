using System;

namespace AsyncListeners
{
    public class AsyncRepeatingListener : AsyncListener
    {
        public delegate void ListenEvent();

        public event ListenEvent OnTrigger;

        private float _t;
        
        public AsyncRepeatingListener(Func<bool> listener) : base(listener)
        {
            
        }

        internal override void Listen()
        {
            if (Listener.Invoke())
            {
                Trigger();
            }
        }

        private void Trigger()
        {
            if (OnTrigger != null) OnTrigger();
        }

        public void Finish()
        {
            IsFinished = true;
        }
    }
}