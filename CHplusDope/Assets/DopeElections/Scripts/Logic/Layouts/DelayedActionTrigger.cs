using System;
using UnityEngine;
using UnityEngine.Events;

namespace DopeElections.Layouts
{
    public class DelayedActionTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onTrigger = new UnityEvent();

        private float _timeout = -1;

        public UnityEvent onTrigger => _onTrigger;

        private void OnEnable()
        {
            if (_timeout <= 0) enabled = false;
        }

        private void OnDisable()
        {
            _timeout = -1;
        }

        private void Update()
        {
            _timeout -= Time.deltaTime;
            if (_timeout <= 0)
            {
                _onTrigger.Invoke();
                enabled = false;
            }
        }

        public void Trigger(float delay)
        {
            _timeout = delay;
            enabled = true;
        }
    }
}