using DopeElections.Races.States;
using FMODSoundInterface;
using StateMachines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.Races
{
    public class ImpactObstacleController : RaceObstacleController<ImpactObstacle>
    {
        private static readonly int IdleOffsetProperty = Animator.StringToHash("_idleOffset");
        
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _meshTransform = null;
        [SerializeField] private Transform _impactOrigin = null;
        [SerializeField] private float _impactDelay = 0.5f;
        [SerializeField] private StateMachine _stateMachine = null;

        private StateMachine StateMachine => _stateMachine;
        private float _t = 0;
        private int _impactCount = 0;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            var course = Obstacle.Course;
            var tileSize = course.Configuration.TileSize;
            var size = Obstacle.Size;
            var pivot = Obstacle.Type.GetPivot(Obstacle.Anchor);
            var meshPosition = new Vector3(size.x * tileSize * pivot.x, 0, size.y * tileSize * pivot.y);
            _meshTransform.localPosition = meshPosition;
            _meshTransform.localRotation = Obstacle.Anchor == ImpactObstacle.TrackAnchor.Left
                ? Quaternion.Euler(0, 180, 0)
                : Quaternion.identity;
            _animator.SetFloat(IdleOffsetProperty, Random.value);
            
            Idle();
        }

        private void Update()
        {
            _t += Time.deltaTime;
            StateMachine.Run();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            StateMachine.State = null;
        }

        public void Smash()
        {
            _impactCount++;
            StateMachine.State = new ImpactObstacleSmashState(this, _impactDelay, Obstacle.Type.Duration);
        }

        public void Idle()
        {
            var waitTime = Obstacle.Delay + _impactCount * Obstacle.Type.Frequency - _impactDelay - _t;
            StateMachine.State = new ImpactObstacleIdleState(this, waitTime);
        }

        public override void PlayAppearAnimation()
        {
            _animator.Play("appear");
        }

        public void PlayIdleAnimation()
        {
            
        }

        public void PlaySmashAnimation()
        {
            _animator.CrossFade("smash", 0.05f);
            SoundController.Play(Obstacle.Type.ImpactSound);
        }

        public void PlayImpactAnimation()
        {
            RaceController.CameraController.Shake(Obstacle.Type.ImpactStrength, 2);
            
            var effect = Obstacle.Type.ImpactEffect;
            if (_impactOrigin && effect)
            {
                var parent = _impactOrigin;
                var reference = RaceController.RaceTrackController.Root;
                RaceController.EffectsController.PlayEffect(effect, _impactOrigin, reference);
            }
        }
    }
}