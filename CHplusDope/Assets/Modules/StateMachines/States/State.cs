using System;

namespace StateMachines
{
    public abstract class State
    {
        public delegate void StateEvent();

        public event StateEvent OnCompleted;
        public event StateEvent OnCancelled;
        public event StateEvent OnFinished;
        
        public bool IsInitialized { get; private set; }
        public bool IsCompleted { get; protected set; }
        public bool IsCancelled { get; private set; }
        public bool IsFinished { get; internal set; }
        
        public void Initialize()
        {
            IsInitialized = true;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }

        public abstract void Update();

        internal void Complete()
        {
            OnComplete();
            if (OnCompleted != null) OnCompleted();
        }
        
        protected virtual void OnComplete(){}

        internal void Cancel()
        {
            IsCancelled = true;
            OnCancel();
            if (OnCancelled != null) OnCancelled();
        }
        
        protected virtual void OnCancel(){}

        internal void Finish()
        {
            IsFinished = true;
            OnFinish();
            if (OnFinished != null) OnFinished();
        }
        
        protected virtual void OnFinish(){}

        public void Then(Action resolve, Action reject = null)
        {
            OnCompleted += resolve.Invoke;
            if (reject != null) OnCancelled += reject.Invoke;
        }

        public void After(Action callback)
        {
            OnFinished += callback.Invoke;
        }
    }
}