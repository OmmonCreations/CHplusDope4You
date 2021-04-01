using System;
using UnityEngine;

namespace StateMachines
{
    public class DelayedActionState : State
    {
        private Action Action { get; }
        private float Delay { get; }

        private float _t;

        public DelayedActionState(Action action, float delay) : base()
        {
            Action = action;
            Delay = delay;
        }

        public override void Update()
        {
            _t += Time.deltaTime;
            if (_t < Delay) return;

            try
            {
                if (Action != null) Action();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                IsCompleted = true;
            }
        }
    }
}