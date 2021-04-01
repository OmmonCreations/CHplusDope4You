using System;
using System.Linq;
using CameraSystems;
using DopeElections.Progression;
using DopeElections.Sounds;
using FMODSoundInterface;
using FMODUnity;
using RuntimeAssetImporter;
using StateMachines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.ExtraInfoTalkers
{
    public class ExtraInfoTalkerController : MonoBehaviour, ISpatialTargetable, IAsset
    {
        [SerializeField] private int _id = 0;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Transform _speechBubbleAnchor = null;
        [SerializeField] private CameraTransformation _cameraTransformation = default;
        [SerializeField] private ParticleSystem _disappearParticles = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private float _approachTime = 1f;
        [SerializeField] private float _disappearTime = 1f;
        [SerializeField] [EventRef] private string _approachSound = null;
        [SerializeField] [EventRef] private string _talkSound = null;
        [SerializeField] [EventRef] private string _disappearSound = Sound.Sfx.Particle.ExplosionConfetti;

        public IExtraInfoEntry Entry { get; private set; }

        public int Key => _id;
        public Vector3 Position => _speechBubbleAnchor.position;
        public Quaternion Rotation => _speechBubbleAnchor.rotation;
        public float Height => 0;

        private StateMachine StateMachine => _stateMachine;

        private string _animation;
        private float _normalizedTime;
        private bool _animationPlayed = false;

        public void Initialize(IExtraInfoEntry entry)
        {
            Entry = entry;
            PlayIdleAnimation();
        }

        private void OnEnable()
        {
            UpdateAnimation();
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void PlayIdleAnimation()
        {
            PlayAnimation("idle", Random.value);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public State Approach()
        {
            PlayApproachAnimation();
            var result = new DelayedActionState(null, _approachTime);
            StateMachine.State = result;
            return result;
        }

        public State Disappear()
        {
            PlayDisappearAnimation();
            var result = new DelayedActionState(Remove, _disappearTime);
            StateMachine.State = result;
            return result;
        }

        private void PlayAnimation(string animation, float normalizedTime = 0)
        {
            _animation = animation;
            _normalizedTime = normalizedTime;
            _animationPlayed = false;
            UpdateAnimation();
        }

        public void PlayApproachAnimation()
        {
            PlayAnimation("approach");
            if (_approachSound != null) SoundController.Play(_approachSound);
        }

        public void PlayTalkAnimation()
        {
            if (_talkSound != null) SoundController.Play(_talkSound);
        }

        public void PlayDisappearAnimation()
        {
            PlayAnimation("disappear");
            _disappearParticles.Play();
            _animator.gameObject.SetActive(false);
            if (_disappearSound != null) SoundController.Play(_disappearSound);
        }

        private void UpdateAnimation()
        {
            if (_animationPlayed) return;
            if (!gameObject.activeInHierarchy) return;
            _animationPlayed = true;
            if (_animator.HasState(0, Animator.StringToHash(_animation)))
            {
                _animator.Play(_animation, 0, _normalizedTime);
            }
        }

        public CameraTransformation CreateCameraTransformation()
        {
            return transform.Transform(_cameraTransformation);
        }
    }
}