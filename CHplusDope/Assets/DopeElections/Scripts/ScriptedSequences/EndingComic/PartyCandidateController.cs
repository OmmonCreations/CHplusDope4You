using DopeElections.Answer;
using DopeElections.Candidates;
using StateMachines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class PartyCandidateController : MonoBehaviour
    {
        private StateMachine _stateMachine = null;

        private PartyCandidateAnimations _animations = null;
        private CandidateController _candidateController = null;

        private float _jumpHeight;

        public Candidate Candidate
        {
            get => _candidateController.Candidate;
            set => _candidateController.Candidate = value;
        }

        public float JumpHeight
        {
            get => _jumpHeight;
            set => _jumpHeight = value;
        }

        private StateMachine StateMachine => _stateMachine;

        public void Initialize(PartyCandidateAnimations animations, CandidateController candidateController)
        {
            _animations = animations;
            _candidateController = candidateController;
            _stateMachine = gameObject.AddComponent<StateMachine>();
        }

        private void OnEnable()
        {
            PlayNextAction();
        }

        private void Update()
        {
            StateMachine.Run();
        }

        public void PlayNextAction()
        {
            var action = Random.value;
            if (action < 0.2f && JumpHeight > 0)
            {
                Jump();
            }
            else if (action < 0.4f && JumpHeight > 0)
            {
                Throw();
            }
            else
            {
                Dance();
            }
        }

        private void Jump()
        {
            StateMachine.State = new JumpState(this, JumpHeight + Random.Range(-JumpHeight * 0.2f, JumpHeight * 0.2f));
        }

        private void Throw()
        {
            StateMachine.State = new ThrowState(this, JumpHeight + Random.Range(-JumpHeight * 0.4f, JumpHeight * 0.4f));
        }

        private void Dance()
        {
            StateMachine.State = new DanceState(this);
        }

        public void PlayJumpAnimation(float height)
        {
            _candidateController.Jump(height, GetJumpTime(height), _animations.JumpArcCurve, false);
        }

        public void PlayThrowAnimation(float height)
        {
            _candidateController.Throw(height, GetJumpTime(height), _animations.JumpArcCurve, false);
        }

        public void PlayDanceAnimation()
        {
            _candidateController.PlayCelebrationAnimation(Random.value, false);
        }

        public float GetJumpTime(float height)
        {
            return Mathf.Lerp(0.1f, 1f, Mathf.Clamp01(height / 5));
        }
    }
}