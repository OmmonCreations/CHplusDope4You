using DopeElections.Candidates;
using DopeElections.Races.States;
using FMODSoundInterface;
using Navigation;
using StateMachines;
using UnityEngine;

namespace DopeElections.Races
{
    public class CannonObstacleController : RaceObstacleController<CannonObstacle>, ICandidateAnchor
    {
        [SerializeField] private Transform _meshTransform = null;
        [SerializeField] private Transform _barrelTransform = null;
        [SerializeField] private Vector3 _barrelLoadAngle = new Vector3(90, 0, 0);
        [SerializeField] private Vector3 _barrelAimAngle = new Vector3(45, 0, 0);
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _candidateAnchor = null;
        [SerializeField] private StateMachine _stateMachine = null;

        public Transform CandidateAnchor => _candidateAnchor;
        public Transform BarrelTransform => _barrelTransform;
        public Vector3 BarrelLoadAngle => _barrelLoadAngle;
        public Vector3 BarrelAimAngle => _barrelAimAngle;
        public Quaternion BarrelRotation { get; set; }
        
        private StateMachine StateMachine => _stateMachine;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var size = Obstacle.Size;
            var tileSize = Obstacle.TileSize;

            Obstacle.LoadStarted += OnCannonLoadStarted;
            Obstacle.Shot += OnCannonShot;

            BarrelRotation = BarrelTransform.localRotation;

            _meshTransform.localPosition = new Vector3(size.x, 0, size.y) * tileSize / 2f;
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void LateUpdate()
        {
            BarrelTransform.localRotation = BarrelRotation;
        }

        public override void PlayAppearAnimation()
        {
            _animator.Play("appear");
        }

        public void PlayLoadAnimation()
        {
            _animator.CrossFade("load", 0.1f);
            SoundController.Play(Obstacle.Type.LoadSound);
        }

        public void PlayShootAnimation()
        {
            _animator.CrossFade("shoot", 0.1f);
            SoundController.Play(Obstacle.Type.ShootSound);
        }

        public void PlayShootEffect()
        {
            var effect = Obstacle.Type.ShootEffect;
            if (effect == null) return;
            
            var reference = RaceController.RaceTrackController.Root;
            RaceController.EffectsController.PlayEffect(Obstacle.Type.ShootEffect, CandidateAnchor, reference);
            RaceController.CameraController.Shake(Obstacle.Type.ImpactStrength, 2);
        }

        private void OnCannonLoadStarted(INavigationAgent agent)
        {
            var loadState = new LoadCannonState(this, Obstacle.LoadTime);
            var aimState = new AimCannonState(this, Obstacle.AimTime);
            var shootState = new ShootCannonState(this, Obstacle.Type.ShootDelay);
            loadState.OnCompleted += () => StateMachine.State = aimState;
            aimState.OnCompleted += () => StateMachine.State = shootState;
            StateMachine.State = loadState;
        }

        private void OnCannonShot(INavigationAgent agent)
        {
            // do something?
        }
    }
}