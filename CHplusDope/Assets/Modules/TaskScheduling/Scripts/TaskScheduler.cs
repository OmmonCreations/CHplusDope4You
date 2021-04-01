using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskScheduling
{
    public class TaskScheduler : MonoBehaviour
    {
        private List<DelayedTask> Queue { get; } = new List<DelayedTask>();

        private List<DelayedTask> Done { get; } = new List<DelayedTask>();
        private void Update()
        {
            foreach (var t in Queue.Where(t => t.ExecutionTime <= Time.time && !t.IsCompleted).ToList())
            {
                t.Run();
                if (!t.IsCompleted) continue;
                Done.Add(t);
            }

            if (Done.Count > 0)
            {
                Queue.RemoveAll(t => Done.Contains(t) || t.IsCompleted);
                Done.Clear();
            }
        }

        public DelayedTask RunTaskLater(Action callback, float delay)
        {
            var result = new DelayedTask(callback, Time.time, Time.time + delay);
            Queue.Add(result);
            return result;
        }

        public DelayedRepeatingTask RunTaskTimer(Action callback, float delay, float interval)
        {
            var result = new DelayedRepeatingTask(callback, Time.time, Time.time + delay, interval);
            Queue.Add(result);
            return result;
        }

        public DelayedRepeatingTask Await(Func<bool> condition, Action callback)
        {
            var result = new DelayedRepeatingTask(condition, callback, Time.time, Time.time, 1f / Application.targetFrameRate);
            
            Queue.Add(result);
            return result;
        }

        public void CancelAll()
        {
            Queue.Clear();
        }

        public static TaskScheduler Create()
        {
            var result = new GameObject("TaskScheduler", typeof(TaskScheduler)).GetComponent<TaskScheduler>();
            DontDestroyOnLoad(result);
            return result;
        }
    }
}
