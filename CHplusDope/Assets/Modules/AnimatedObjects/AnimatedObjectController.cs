using StateMachines;
using UnityEngine;
using UnityEngine.Events;

namespace AnimatedObjects
{
    public abstract class AnimatedObjectController : MonoBehaviour
    {
        [SerializeField] private StateMachine _stateMachine = null;

        protected StateMachine StateMachine => _stateMachine;

        protected void Update()
        {
            StateMachine.Run();
        }

        protected void OnDestroy()
        {
        }

        protected virtual void OnDestroyed()
        {
        }
    }
}