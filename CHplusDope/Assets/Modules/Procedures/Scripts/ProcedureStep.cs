namespace Procedures
{
    public abstract class ProcedureStep
    {
        internal delegate void StepEvent();

        internal event StepEvent Completed;
        internal event StepEvent Cancelled;

        private bool _finished;

        internal bool IsFinished => _finished;
        
        public abstract void Run();

        protected void Complete()
        {
            if (_finished) return;
            _finished = true;
            if (Completed != null) Completed();
            OnFinish();
        }

        public void Cancel()
        {
            if (_finished) return;
            _finished = true;
            if (Cancelled!=null) Cancelled();
            OnCancelled();
            OnFinish();
        }
        
        protected virtual void OnCancelled(){}
        
        protected virtual void OnFinish(){}
    }
    
    public abstract class ProcedureStep<T>
    {
        internal delegate void StepEvent(T data);

        internal event StepEvent Completed;
        internal event StepEvent Cancelled;

        private bool _finished;

        internal bool IsFinished => _finished;
        
        public abstract void Run(T data);

        protected void Complete(T data)
        {
            if (_finished) return;
            _finished = true;
            if (Completed != null) Completed(data);
            OnFinish(data);
        }

        public void Cancel(T data)
        {
            if (_finished) return;
            _finished = true;
            if (Cancelled!=null) Cancelled(data);
            OnCancelled(data);
            OnFinish(data);
        }
        
        protected virtual void OnCancelled(T data){}
        
        protected virtual void OnFinish(T data){}
    }
}