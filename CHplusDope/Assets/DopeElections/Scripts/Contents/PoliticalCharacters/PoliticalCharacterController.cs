using System.Collections.Generic;
using DopeElections.Candidates;
using DopeElections.Sounds;
using Essentials.LOD;
using FMODSoundInterface;
using Localizator;
using SpeechBubbles;
using StateMachines;
using UnityEngine;

namespace DopeElections.PoliticalCharacters
{
    public abstract class PoliticalCharacterController : MonoBehaviour, ISpatialTargetable
    {
        #region Static Fields

        private static readonly int MovementSpeedProperty = Animator.StringToHash("_movementSpeed");
        private static readonly int FallSpeedProperty = Animator.StringToHash("_fallSpeed");
        private static readonly int JumpSpeedProperty = Animator.StringToHash("_jumpSpeed");

        private static readonly Dictionary<Color, Material> MaterialCache = new Dictionary<Color, Material>();

        #endregion

        #region Serialized Fields

        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private SkinnedMeshRenderer _bodyRenderer = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private SpeechBubbleAnchor _speechBubbleAnchor = null;
        [SerializeField] private LODSwitcher _lodSwitcher = null;

        #endregion

        #region Private Fields

        private Transform _transform;
        private Color _bodyColor;
        private float _movementSpeed;

        #endregion

        #region Properties

        public IPoliticalCharacterEnvironment Environment { get; private set; }
        public SpeechBubbleAnchor SpeechBubbleAnchor => _speechBubbleAnchor;

        public new Transform transform
        {
            get
            {
                if (!_transform) _transform = base.transform;
                return _transform;
            }
        }

        public Color BodyColor
        {
            get => _bodyColor;
            set => ApplyBodyColor(value);
        }

        public float MovementSpeed
        {
            get => _movementSpeed;
            set => ApplyMovementSpeed(value);
        }

        public int LOD
        {
            get => _lodSwitcher.level;
            set => _lodSwitcher.level = value;
        }

        #endregion

        #region Auto Properties

        public string Id => name;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public float Height => transform.InverseTransformVector(new Vector3(0, 2, 0)).magnitude;
        public float PreferredFocusDistance => 10;
        protected StateMachine StateMachine => _stateMachine;
        protected SkinnedMeshRenderer BodyRenderer => _bodyRenderer;

        #endregion

        #region Initializers

        public void Initialize(IPoliticalCharacterEnvironment environment = null)
        {
            Environment = environment;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        #endregion

        #region Unity Control

        private void Update()
        {
            StateMachine.Run();
        }

        protected void OnDestroy()
        {
            OnDestroyed();
        }

        protected virtual void OnDestroyed()
        {
        }

        #endregion

        #region Public API

        #region Commands

        public void Remove()
        {
            Destroy(gameObject);
        }

        #region Say

        public SpeechBubbleController Say(LocalizationKey key)
        {
            var text = Environment.Localization.GetString(key);
            return Say(text);
        }

        public SpeechBubbleController<TextSpeechBubble> Say(TextSpeechBubbleController prefab, LocalizationKey key)
        {
            var text = Environment.Localization.GetString(key);
            return Say(prefab, text);
        }

        public SpeechBubbleController<TextSpeechBubble> Say(string text)
        {
            return Say(
                new TextSpeechBubble(_speechBubbleAnchor ? (ISpatialTargetable) _speechBubbleAnchor : this, text));
        }

        public TextSpeechBubbleController Say(TextSpeechBubbleController prefab, string text)
        {
            return Say(prefab,
                new TextSpeechBubble(_speechBubbleAnchor ? (ISpatialTargetable) _speechBubbleAnchor : this, text));
        }

        public SpeechBubbleController<T> Say<T>(T speechBubble) where T : SpeechBubble
        {
            SoundController.Play(Sound.Sfx.Character.Talk);
            return Environment.SpeechBubbleSource.ShowSpeechBubble<T, SpeechBubbleController<T>>(speechBubble);
        }

        public T2 Say<T, T2>(T2 prefab, T speechBubble) where T : SpeechBubble where T2 : SpeechBubbleController<T>
        {
            SoundController.Play(Sound.Sfx.Character.Talk);
            return Environment.SpeechBubbleSource.ShowSpeechBubble(prefab, speechBubble);
        }

        #endregion

        public void Idle()
        {
            StateMachine.State = null;
            PlayIdleAnimation();
        }

        public void IdleLookingUp()
        {
            StateMachine.State = null;
            PlayIdleLookingUpAnimation();
        }

        public RunToPositionState RunTo(Vector3 position, float speed)
        {
            var state = new RunToPositionState(this, position, speed);
            StateMachine.State = state;
            return state;
        }

        #endregion

        #region Animations

        public void PlayIdleAnimation()
        {
            if (_animator && _animator.gameObject.activeInHierarchy) _animator.CrossFade("idle", 0.1f, 0);
        }

        public void Jump(float height, float time, AnimationCurve arcCurve, bool playSound = true)
        {
            StateMachine.State = new JumpState(this, "jump", height, time, arcCurve);
            if (playSound) SoundController.Play(Sound.Sfx.Character.Jump);
        }

        public void JumpHurdle100(float height, float time, AnimationCurve arcCurve)
        {
            StateMachine.State = new JumpState(this, "hurdle-jump-100", height, time, arcCurve);
        }

        public void JumpHurdle75(float height, float time, AnimationCurve arcCurve)
        {
            StateMachine.State = new JumpState(this, "hurdle-jump-75", height, time, arcCurve);
        }

        public void JumpHurdle50(float height, float time, AnimationCurve arcCurve)
        {
            StateMachine.State = new JumpState(this, "hurdle-jump-50", height, time, arcCurve);
        }

        public void JumpHurdle25(float height, float time, AnimationCurve arcCurve)
        {
            StateMachine.State = new JumpState(this, "hurdle-jump-25", height, time, arcCurve);
        }

        public void Throw(float height, float time, AnimationCurve arcCurve, bool playSound = true)
        {
            StateMachine.State = new ThrowState(this, height, time, arcCurve);
            if (playSound) SoundController.Play(Sound.Sfx.Character.Jump);
        }

        public void PlayAttachAnimation(ICandidateAnchor anchor, float height, float time, AnimationCurve timeCurve,
            AnimationCurve arcCurve)
        {
            StateMachine.State = new AttachState(this, anchor, height, time, timeCurve, arcCurve);
        }

        public void PlayIdleLookingUpAnimation()
        {
            if (_animator && _animator.gameObject.activeInHierarchy) _animator.CrossFade("idle-looking-up", 0.1f, 0);
        }

        public void PlayRunningAnimation()
        {
            if (_animator && _animator.gameObject.activeInHierarchy)
                _animator.CrossFade("run-slow", 0.1f, 0, Random.value);
        }

        public void PlayBoostAnimation()
        {
            if (_animator && _animator.gameObject.activeInHierarchy) _animator.CrossFade("boost", 0.1f, 0);
            SoundController.Play(Sound.Sfx.Character.Boost);
        }

        public void PlayCelebrationAnimation() => PlayCelebrationAnimation(0);

        public void PlayCelebrationAnimation(float normalizedTime) => PlayCelebrationAnimation(normalizedTime, true);

        public void PlayCelebrationAnimation(float normalizedTime, bool playSound)
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.CrossFade("celebrate-" + Random.Range(1, 6), 0.1f, 0, Random.value);
            if (playSound) SoundController.Play(Sound.Sfx.Character.YellHappy);
        }

        public void PlayJumpAnimation(float animationTime = 0.6f)
        {
            PlayJumpAnimation("jump", animationTime);
        }

        public void PlayJumpAnimation(string animationId, float animationTime = 0.6f)
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.SetFloat(JumpSpeedProperty, 0.4f / animationTime);
            _animator.CrossFade(animationId, 0.1f, 0);
        }

        public void PlayThrowAnimation(float animationTime = 0.6f)
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.SetFloat(JumpSpeedProperty, 0.4f / animationTime);
            _animator.CrossFade("throw", 0.1f, 0);
        }

        public void PlayFallAnimation(float time)
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.SetFloat(FallSpeedProperty, 1 / time);
            _animator.CrossFade("fall", 0.1f, 0);
            SoundController.Play(Sound.Sfx.Character.Fall);
        }

        public void PlayFallRecoveryAnimation()
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.CrossFade("fall-recovery", 0.1f, 0);
        }

        public void PlaySquishAnimation()
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.CrossFade("squish", 0.1f, 0);
            SoundController.Play(Sound.Sfx.Character.Squish);
        }

        public void PlaySquishWithRecoveryAnimation()
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy) return;
            _animator.CrossFade("squish-recovery", 0.1f, 0);
            SoundController.Play(Sound.Sfx.Character.Squish);
        }

        #endregion

        #endregion

        #region Data Modifiers

        private void ApplyMovementSpeed(float speed)
        {
            _movementSpeed = speed;
            if (_animator && _animator.gameObject.activeInHierarchy) _animator.SetFloat(MovementSpeedProperty, speed);
        }

        private void ApplyBodyColor(Color color)
        {
            _bodyColor = color;
            if (!MaterialCache.TryGetValue(color, out var material))
            {
                material = Instantiate(_bodyRenderer.sharedMaterials[0]);
                material.color = color;
                MaterialCache.Add(color, material);
            }

            var materials = _bodyRenderer.sharedMaterials;
            materials[0] = material;
            _bodyRenderer.sharedMaterials = materials;
        }

        #endregion
    }
}