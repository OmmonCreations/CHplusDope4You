using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Procedures
{
    public class Procedure
    {
        private ProcedureStep[] Steps { get; }
        private bool IsCancelled { get; set; }

        private bool _running;
        private Action _callback;
        private ProcedureStep _current;

        public Procedure(IEnumerable<ProcedureStep> steps)
        {
            Steps = steps.ToArray();
        }
        
        public Procedure(params ProcedureStep[] steps)
        {
            Steps = steps;
        }

        public void Run(Action callback)
        {
            if (_running)
            {
                Debug.LogError("Procedure is already running!");
                return;
            }

            _running = true;
            _callback = callback;
            
            var steps = Steps;
            if (steps.Length == 0)
            {
                callback();
                return;
            }
            for (var i = 0; i < steps.Length-1; i++)
            {
                var nextStep = i + 1;
                steps[i].Completed += () =>
                {
                    if (IsCancelled) return;

                    var next = steps[nextStep];
                    _current = next;
                    next.Run();
                };
                steps[i].Cancelled += () =>
                {
                    if (IsCancelled) return;
                    IsCancelled = true;
                    _running = false;
                    callback();
                };
            }
            
            steps[steps.Length-1].Completed += () =>
            {
                _running = false;
                callback();
            };
            
            steps[steps.Length-1].Cancelled += () =>
            {
                if (IsCancelled) return;
                IsCancelled = true;
                _running = false;
                callback();
            };

            _current = steps[0];
            steps[0].Run();
        }

        public void Cancel()
        {
            if (!_running || IsCancelled) return;
            IsCancelled = true;
            _running = false;
            _callback();
            if(_current!=null) _current.Cancel();
        }
    }
    public class Procedure<T>
    {
        private ProcedureStep<T>[] Steps { get; }
        private bool IsCancelled { get; set; }

        private bool _running;
        private Action<T> _callback;
        private ProcedureStep<T> _current;

        public Procedure(IEnumerable<ProcedureStep<T>> steps)
        {
            Steps = steps.ToArray();
        }
        
        public Procedure(params ProcedureStep<T>[] steps)
        {
            Steps = steps;
        }

        public void Run(T data, Action<T> callback)
        {
            if (_running)
            {
                Debug.LogError("Procedure is already running!");
                return;
            }

            _running = true;
            _callback = callback;
            
            var steps = Steps;
            if (steps.Length == 0)
            {
                callback(data);
                return;
            }
            for (var i = 0; i < steps.Length-1; i++)
            {
                var nextStep = i + 1;
                steps[i].Completed += postActionData =>
                {
                    if (IsCancelled) return;

                    var next = steps[nextStep];
                    _current = next;
                    next.Run(postActionData);
                };
                steps[i].Cancelled += postActionData =>
                {
                    if (IsCancelled) return;
                    IsCancelled = true;
                    _running = false;
                    callback(postActionData);
                };
            }
            
            steps[steps.Length-1].Completed += postActionData =>
            {
                _running = false;
                callback(postActionData);
            };
            
            steps[steps.Length-1].Cancelled += postActionData =>
            {
                if (IsCancelled) return;
                IsCancelled = true;
                _running = false;
                callback(postActionData);
            };

            _current = steps[0];
            steps[0].Run(data);
        }

        public void Cancel(T data)
        {
            if (!_running || IsCancelled) return;
            IsCancelled = true;
            _running = false;
            if(_current!=null) _current.Cancel(data);
            _callback(data);
        }
    }
}
