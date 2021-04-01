using System;

namespace TaskScheduling
{
    public class DelayedTask
    {
        private Action Callback { get; }
        public float StartTime { get; set; }
        public float ExecutionTime { get; set; }
        public bool IsCompleted { get; protected set; }

        public DelayedTask(Action callback, float startTime, float executionTime)
        {
            Callback = callback;
            StartTime = startTime;
            ExecutionTime = executionTime;
        }

        public virtual void Run()
        {
            Callback.Invoke();
            IsCompleted = true;
        }

        public void Cancel()
        {
            IsCompleted = true;
        }
    }
}
