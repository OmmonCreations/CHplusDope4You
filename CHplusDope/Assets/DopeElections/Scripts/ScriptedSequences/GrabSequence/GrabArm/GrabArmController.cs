using DopeElections.Localizations;
using DopeElections.ScriptedSequences.GrabSequence.Guides;
using MobileInputs;
using MobileInputs.Dragging;
using PopupInfos;
using StateMachines;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DopeElections.ScriptedSequences.GrabSequence
{
    public class GrabArmController : MonoBehaviour, ISpatialTargetable
    {
        public delegate void GrabEvent(bool success);
        public delegate void StateEvent();

        public event GrabEvent Grabbed = delegate { };
        public event StateEvent Disappeared = delegate { };

        [SerializeField] private Animator _animator = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private Transform _playerAnchor = null;
        [SerializeField] private Transform _grabPlane = null;
        [SerializeField] private Color _successColor = Color.green;
        [SerializeField] private Color _missColor = Color.red;
        [SerializeField] private TapGuideController _tapGuide = null;
        [SerializeField] private float _showTapGuideAfter = 30;

        [Header("Position Anchors")] [SerializeField]
        private Transform _outsideViewAnchor = null;

        [SerializeField] private Transform _grabOriginAnchor = null;
        [SerializeField] private Transform _idleAnchor = null;
        [SerializeField] private Transform _grabbedAnchor = null;

        private string _animation;
        private float _tapGuideDelay;
        private bool _tapGuideShown = false;

        private StateMachine StateMachine => _stateMachine;

        public GrabSequenceController SequenceController { get; private set; }
        public GrabSequencePlayerController PlayerController { get; private set; }
        public Transform PlayerAnchor => _playerAnchor;
        public Transform OutsideViewAnchor => _outsideViewAnchor;
        public Transform GrabOriginAnchor => _grabOriginAnchor;
        public Transform IdleAnchor => _idleAnchor;
        public Transform GrabbedAnchor => _grabbedAnchor;
        public TapGuideController TapGuide => _tapGuide;
        public bool CanGrab { get; set; }

        public string Id { get; } = "grab_arm";
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public float Height => 0;
        public float PreferredFocusDistance => 0;

        public void Initialize(GrabSequenceController sequenceController)
        {
            SequenceController = sequenceController;
            TapGuide.Initialize(sequenceController);
            TapGuide.Label.key = LKey.ScriptedSequences.GrabSequence.Tap;
            HookEvents();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            StateMachine.Run();
            if (!_tapGuideShown && CanGrab)
            {
                if (_tapGuideDelay > 0)
                {
                    _tapGuideDelay -= Time.deltaTime;
                    return;
                }

                TapGuide.Follow(PlayerController);
                _tapGuideShown = true;
            }
        }

        private void OnDestroy()
        {
            ReleaseHooks();
        }

        private void HookEvents()
        {
            if(SequenceController) SequenceController.InteractionSystem.OnPointerDown += OnPointerDown;
        }

        private void ReleaseHooks()
        {
            if(SequenceController) SequenceController.InteractionSystem.OnPointerDown -= OnPointerDown;
        }

        private void OnPointerDown(IInteractable interactable, InputAction.CallbackContext context)
        {
            if (!CanGrab) return;
            var input = context.ReadValue<PointerInput>();
            var ray = SequenceController.InteractionSystem.EventCamera.ScreenPointToRay(input.Position);
            var plane = new Plane(_grabPlane.forward, _grabPlane.position);
            if (!plane.Raycast(ray, out var hit))
            {
                return;
            }

            var point = ray.GetPoint(hit);
            var success = interactable is GrabSequencePlayerController;
            Grab(point, success);

            if (!success && !_tapGuideShown) _tapGuideDelay -= 10;
        }

        public void Launch(GrabSequencePlayerController playerController)
        {
            PlayerController = playerController;
            CanGrab = true;
            _tapGuideDelay = _showTapGuideAfter;
            _tapGuideShown = false;
            Appear();
        }

        public AppearState Appear()
        {
            CanGrab = true;
            var state = new AppearState(this);
            StateMachine.State = state;
            return state;
        }

        public IdleState Idle()
        {
            CanGrab = true;
            var state = new IdleState(this);
            StateMachine.State = state;
            return state;
        }

        public GrabState Grab(Vector3 position, bool hit)
        {
            CanGrab = false;
            var state = new GrabState(this, position, hit);
            StateMachine.State = state;

            if (hit) TapGuide.Hide();
            return state;
        }

        public DisappearState Disappear()
        {
            CanGrab = false;
            var state = new DisappearState(this);
            state.OnCompleted += () => Disappeared();
            StateMachine.State = state;
            TapGuide.Hide();
            return state;
        }

        internal void ShowGrabFeedback(bool success)
        {
            var hintKey = success ? LKey.ScriptedSequences.GrabSequence.Success : LKey.ScriptedSequences.GrabSequence.Miss;
            var hint = SequenceController.Localization.GetString(hintKey);
            SequenceController.PopupInfoLayer.Show(this, new PopupInfoData
            {
                Color = success ? _successColor : _missColor,
                Text = hint
            });
        }

        internal void TriggerGrabbed(bool success)
        {
            Grabbed(success);
        }

        public void PlayIdleAnimation()
        {
            PlayAnimation("idle");
        }

        public void PlayGrabAnimation()
        {
            PlayAnimation("grab");
        }

        public void PlayGrabEmptyAnimation()
        {
            PlayAnimation("grab-empty");
        }

        private void PlayAnimation(string animation)
        {
            if (animation == _animation) _animator.Play(animation, -1, 0);
            else _animator.CrossFade(animation, 0.1f, -1);
            _animation = animation;
        }
    }
}