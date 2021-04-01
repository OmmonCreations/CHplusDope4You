using UnityEngine;

namespace StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        private State _state = null;

        [SerializeField] private string _currentStateName = null;
        
        public State State
        {
            get => _state;
            set => ApplyState(value);
        }

        private void OnDestroy()
        {
            State = null;
        }

        private void OnApplicationQuit()
        {
            State = null;
        }
        
        private void ApplyState(State state)
        {
            var active = _state;
            if (state != active && active != null)
            {
                if (!active.IsFinished)
                {
                    active.IsFinished = true;
                    if(active.IsCompleted) active.Complete();
                    else active.Cancel();
                    active.Finish();
                }
            }
            _state = state;
            if(state!=null) state.Initialize();
            _currentStateName = state != null ? state.GetType().Name : "";
        }
        
        public void Run()
        {
            var current = _state;

            if (current == null) return;
            if(!current.IsInitialized) current.Initialize();
            if(!current.IsCancelled) current.Update();
            if (current.IsCompleted)
            {
                if (!current.IsFinished)
                {
                    current.IsFinished = true;
                    current.Complete();
                    current.Finish();
                }
            }

            if (current.IsFinished && _state == current)
            {
                ApplyState(null);
            }
        }

        public void Cancel()
        {
            State = null;
        }
    }
}