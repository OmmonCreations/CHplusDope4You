using DopeElections.Candidates;
using DopeElections.ObstacleCourses;
using DopeElections.Races.RaceTracks;
using DopeElections.Sounds;
using Effects;
using FMODSoundInterface;
using Navigation;
using StateMachines;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace DopeElections.Races
{
    public class RaceCandidateController : MonoBehaviour, INavigationAgent, ICandidateAnchor
    {
        public delegate void ActionEvent(INavigationAction action);

        public event ActionEvent ActionStarted = delegate { };
        public event ActionEvent ActionStopped = delegate { };
        public event ActionEvent NavigationStopped = delegate { };

        [FormerlySerializedAs("_candidate")] [SerializeField]
        private CandidateController _candidateController = null;

        [SerializeField] private Transform _candidateAnchor = null;

        [SerializeField] private GlowEffect _glowEffect = null;
        [SerializeField] private RaceCandidateAnimations _animations = null;
        [SerializeField] private RaceCandidateEffects _effects = null;
        [SerializeField] private StateMachine _stateMachine = null;
        #if UNITY_EDITOR
        [Header("Debug Info")]
        [SerializeField] private int _lastAgreement = 0;
        [SerializeField] private bool _isAlive = false;
        [SerializeField] private bool _wasAlive = false;
        [SerializeField] private Vector2Int _slot = default;
        [SerializeField] private Vector2Int _from = default;
        [SerializeField] private Vector2Int _to = default;
        #endif

        private RaceCandidate _candidate;

        private Transform _transform;
        private Vector3 _worldPosition;
        private Quaternion _rotation;
        private RaceTrackVector _position;

        /// <summary>
        /// The current random offset the candidate should keep to the group anchor
        /// </summary>
        private RaceTrackVector _offset;

        /// <summary>
        /// The layout for which the random offset was calculated
        /// </summary>
        private CandidateGroupLayout _offsetLayout;

        private float _currentMovementSpeed;

        public StateMachine StateMachine => _stateMachine;
        public RaceCandidate Candidate => _candidate;
        public CandidateReactionContext LastReactionContext => Candidate.LastReactionContext;
        public CandidateGroup Group => RaceController ? RaceController.CandidateGroup : null;
        public RaceCandidateAnimations Animations => _animations;
        public RaceCandidateEffects Effects => _effects;
        public bool IsVisible => gameObject.activeSelf && _candidateController.gameObject.activeSelf;
        public EffectsController EffectsController => RaceController.EffectsController;
        public GlowEffect GlowEffect => _glowEffect;
        public float BaseSpeed => Candidate.BaseSpeed;
        public Transform CandidateAnchor => _candidateAnchor;
        public Transform CandidateTransform => _candidateController.transform;

        public new Transform transform => _transform;

        public RaceController RaceController { get; private set; }
        public RaceObstacleCourseController ObstacleCourseController { get; set; }
        public INavigationAction CurrentAction { get; private set; }

        public float CurrentMovementSpeed
        {
            get => _currentMovementSpeed;
            set => ApplyCurrentMovementSpeed(value);
        }

        public int LOD
        {
            get => _candidateController.LOD;
            set => _candidateController.LOD = value;
        }

        public float Speed { get; set; }

        public Vector3 WorldPosition
        {
            get => _worldPosition;
            set => ApplyEuclidianPosition(value);
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set => ApplyRotation(value);
        }

        /// <summary>
        /// Race track position where x represents the position perpendicular to the movement direction
        /// and y represents is aligned with the movement direction
        /// </summary>
        public RaceTrackVector Position
        {
            get => _position;
            set => ApplyPosition(value);
        }

        public RaceTrackVector Offset
        {
            get => _offset;
        }

        public void Initialize(RaceController raceController, RaceCandidate candidate)
        {
            _candidateController.Initialize(raceController);

            RaceController = raceController;
            _transform = base.transform;
            _candidate = candidate;
            _candidateController.Candidate = candidate.Candidate;
            HookEvents();
            _glowEffect.Color = Color.clear;
            _glowEffect.Camera = raceController.CameraController.Camera;

            gameObject.name = candidate.fullName;
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void OnDestroy()
        {
            ReleaseHooks();
        }

        #region Event Hooks

        private void HookEvents()
        {
            if (Candidate != null)
            {
                Candidate.FollowedGroup += OnCandidateFollowedGroup;
                Candidate.Reacted += OnCandidateReacted;
                Candidate.Resetted += OnCandidateResetted;
            }

            if (Group != null)
            {
                Group.LayoutChanged += OnLayoutChanged;
            }
        }

        private void ReleaseHooks()
        {
            if (Candidate != null)
            {
                Candidate.FollowedGroup -= OnCandidateFollowedGroup;
                Candidate.Reacted -= OnCandidateReacted;
                Candidate.Resetted -= OnCandidateResetted;
            }

            if (Group != null)
            {
                Group.LayoutChanged -= OnLayoutChanged;
            }
        }

        #endregion

        #region Event Listeners

        private void OnCandidateReacted(GroupReactionContext context, ReactionData data, CompiledPath path)
        {
            if (!this)
            {
                ReleaseHooks();
                return;
            }

            var layout = context.Layout;
            _offset = data.IsAlive ? layout.CreateOffset(data.Slot) : default;
            _offsetLayout = layout;
            if (!data.IsAlive && !data.WasAlive)
            {
                SetActive(false);
                return;
            }

            var state = new ClearObstacleCourseState(this, context, data, path);
            SetActive(true);
            StateMachine.State = state;
        }

        private void OnCandidateFollowedGroup(CandidateGroup group)
        {
            SetActive(Candidate.IsAlive);

            var state = new MoveInGroupState(this, group);
            if (_offsetLayout == null) OnLayoutChanged(group.Layout);
            StateMachine.State = state;
        }

        private void OnCandidateResetted()
        {
            SetActive(Candidate.IsAlive);
            Position = _candidate.GroupAnchor;
            WorldPosition = RaceController.RaceTrack.GetWorldPosition(Position);
            _candidateController.gameObject.SetActive(true);
        }

        private void OnLayoutChanged(CandidateGroupLayout layout)
        {
            if (_offsetLayout == layout) return;
            _offset = layout.CreateOffset(Candidate.Slot);
            _offsetLayout = layout;
        }

        #endregion

        #region Navigation Actions

        public void StartAction(INavigationAction action)
        {
            action.Start(this);
            ActionStarted(action);
            CurrentAction = action;
        }

        public void StopAction(INavigationAction action)
        {
            action.Stop(this);
            ActionStopped(action);
        }

        public void StopNavigation(INavigationAction lastAction)
        {
            NavigationStopped(lastAction);
        }

        #endregion

        #region Commands

        public void Spawn()
        {
            Effects.PlaySpawnParticles();
            _candidateController.gameObject.SetActive(true);
            _candidateController.IdleLookingUp();
            WorldPosition = RaceController.RaceTrack.GetWorldPosition(Position);
            SoundController.Play(Sound.Sfx.Particle.ExplosionConfetti);
        }

        public void Hide()
        {
            Effects.PlayDespawnParticles();
            _candidateController.gameObject.SetActive(false);
        }

        public void HideImmediate()
        {
            _candidateController.gameObject.SetActive(false);
        }

        #endregion

        #region Animations

        public void PlayIdleAnimation()
        {
            _candidateController.IdleLookingUp();
        }

        public void PlayRunningAnimation()
        {
            _candidateController.PlayRunningAnimation();
        }

        public void PlayBoostAnimation()
        {
            _candidateController.PlayBoostAnimation();
        }

        public void PlayJumpAnimation(float height, float time)
        {
            _candidateController.Jump(height, time, _animations.JumpArcCurve);
        }

        public void PlayHurdleJump100Animation(float height, float time)
        {
            _candidateController.JumpHurdle100(height, time, _animations.JumpArcCurve);
        }

        public void PlayHurdleJump75Animation(float height, float time)
        {
            _candidateController.JumpHurdle75(height, time, _animations.JumpArcCurve);
        }

        public void PlayHurdleJump50Animation(float height, float time)
        {
            _candidateController.JumpHurdle50(height, time, _animations.JumpArcCurve);
        }

        public void PlayHurdleJump25Animation(float height, float time)
        {
            _candidateController.JumpHurdle25(height, time, _animations.JumpArcCurve);
        }

        public void PlayFallAnimation(float time)
        {
            _candidateController.PlayFallAnimation(time);
        }

        public void PlayFallRecoveryAnimation()
        {
            _candidateController.PlayFallRecoveryAnimation();
        }

        public void PlaySquishAnimation()
        {
            _candidateController.PlaySquishAnimation();
        }

        public void PlaySquishRecoveryAnimation()
        {
            _candidateController.PlaySquishWithRecoveryAnimation();
        }

        public void PlayAttachAnimation(ICandidateAnchor anchor, float jumpHeight, float time, AnimationCurve timeCurve,
            AnimationCurve jumpCurve)
        {
            _candidateController.PlayAttachAnimation(anchor, jumpHeight, time, timeCurve, jumpCurve);
        }

        #endregion

        #region Data Modifiers

        private void ApplyCurrentMovementSpeed(float speed)
        {
            _currentMovementSpeed = speed;
            _candidateController.MovementSpeed = speed;
        }

        private void ApplyEuclidianPosition(Vector3 position)
        {
            _worldPosition = position;
            transform.localPosition = position;
        }

        private void ApplyRotation(Quaternion rotation)
        {
            _rotation = rotation;
            transform.localRotation = rotation;
        }

        private void ApplyPosition(RaceTrackVector position)
        {
            _position = position;
        }

        #endregion

        #region Public API

        public void SetPositionImmediate(RaceTrackVector position)
        {
            Position = position;
            WorldPosition = RaceController.RaceTrack.GetWorldPosition(position);
        }

        public void SetActive(bool alive)
        {
            if (!Application.isPlaying) return;
            gameObject.SetActive(alive);
        }

        #endregion

        #region Editor Stuff

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (!RaceController || Group == null) return;
            var anchor = Candidate.GroupAnchor;
            var localPoint = RaceController.RaceTrack.GetWorldPosition(anchor);
            var worldPoint = RaceController.RaceTrackController.Root.TransformPoint(localPoint);
            var radius = Group.LayoutConfiguration.SlotSize / 4;
            var tangent = Vector3.forward;
            var right = Vector3.Cross(Vector3.up, tangent);
            var a = worldPoint + (tangent + right) * radius;
            var b = worldPoint + (tangent - right) * radius;
            var c = worldPoint + (-tangent + right) * radius;
            var d = worldPoint + (-tangent - right) * radius;
            Handles.color = new Color(0.5f, 1, 0.5f, 0.5f);
            Handles.DrawAAConvexPolygon(a, b, d, c, a);
        }

        public void UpdateDebugInfo()
        {
            _lastAgreement = LastReactionContext.ReactionData.Agreement;
            _isAlive = LastReactionContext.ReactionData.IsAlive;
            _wasAlive = LastReactionContext.ReactionData.WasAlive;
            _slot = new Vector2Int(LastReactionContext.ReactionData.Slot.x,LastReactionContext.ReactionData.Slot.y);
            _from = LastReactionContext.From;
            _to = LastReactionContext.To;
        }

#endif

        #endregion
    }
}