using DopeElections.Users;
using MobileInputs;
using StateMachines;
using UnityEngine;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class GrabSequencePlayerController : MonoBehaviour, IInteractable
    {
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Transform _playerAnchor = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private BoxCollider _movementBox = null;
        [SerializeField] private Transform _grabbedAnchor = null;

        public Transform PlayerAnchor => _playerAnchor;
        public Transform GrabbedAnchor => _grabbedAnchor;
        public BoxCollider MovementBox => _movementBox;

        private StateMachine StateMachine => _stateMachine;
        public PlayerController PlayerController => _playerController;

        private void Update()
        {
            StateMachine.Run();
        }

        public void Float()
        {
            StateMachine.State = new FloatState(this);
        }

        public void Grabbed()
        {
            StateMachine.State = new GrabbedState(this);
        }

        public void TransformTo(Transform target, float time)
        {
            var transform = this.transform;
            
            var startPosition = transform.position;
            var startRotation = transform.rotation;
            var startScale = transform.localScale;
            
            var targetPosition = target.position;
            var targetRotation = target.rotation;
            var targetScale = target.localScale;
            
            var transition = new TransitionState(time, 0, 1);
            transition.OnTransition += t =>
            {
                var progress = Mathf.SmoothStep(0, 1, t);
                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
                transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            };
            transition.OnCompleted += () =>
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                transform.localScale = targetScale;
            };

            StateMachine.State = transition;
        }
    }
}