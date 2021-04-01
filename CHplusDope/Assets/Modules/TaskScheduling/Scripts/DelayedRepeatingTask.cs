using System;

namespace TaskScheduling
{
    public class DelayedRepeatingTask : DelayedTask
    {
        public int TimesExecuted { get; private set; }
        public float Interval { get; }

        private Func<bool> _condition;
        
        public DelayedRepeatingTask(Func<bool> condition, Action callback, float startTime, float executionTime, float interval) : base(callback, startTime, executionTime)
        {
            Interval = interval;
            _condition = condition;
        }
        
        public DelayedRepeatingTask(Action callback, float startTime, float executionTime, float interval) : base(callback, startTime, executionTime)
        {
            Interval = interval;
        }

        public override void Run()
        {
            TimesExecuted++;
            if (_condition != null)
            {
                var completed = _condition.Invoke();
                if(completed) base.Run();
            }
            else
            {
                base.Run();
                IsCompleted = false;
            }
            ExecutionTime = StartTime + (TimesExecuted + 1) * Interval;
        }
    }
}