using CameraSystems;
using DopeElections.HotAirBalloon;
using DopeElections.ScriptedSequences.LandInMainMenu.States;
using DopeElections.Users;
using Localizator;
using SpeechBubbles;
using UnityEngine;

namespace DopeElections.ScriptedSequences.LandInMainMenu
{
    public class LandInMainMenuCinematicController : ScriptedSequenceController
    {
        [SerializeField] private CameraSystem _cameraSystem = null;
        [SerializeField] private HotAirBalloonController _hotAirBalloon = null;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private SpeechBubbleLayer _speechBubbles = null;
        [SerializeField] private LocalizationScope _localizationScope = null;
        
        [Header("Anchors")]
        [SerializeField] private Transform _hotAirBalloonAnchor = null;
        [Header("Reference Points")]
        [SerializeField] private Transform _hotAirBalloonFrom = null;
        [SerializeField] private Transform _hotAirBalloonTo = null;
        [SerializeField] private Transform _playerTo = null;
        [SerializeField] private CameraTransformation _cameraFrom = default;
        [SerializeField] private CameraTransformation _cameraTo = default;
        [Header("Animations")]
        [SerializeField] private AnimationCurve _cameraMovementCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _cameraJumpCurve = AnimationCurve.Linear(0,0,1,0);
        [SerializeField] private AnimationCurve _playerMovementCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _hotAirBalloonMovementCurve = AnimationCurve.Linear(0,0,1,1);

        public CameraSystem CameraSystem => _cameraSystem;
        public HotAirBalloonController HotAirBalloon => _hotAirBalloon;
        public PlayerController Player => _playerController;
        
        public SpeechBubbleLayer SpeechBubbles => _speechBubbles;
        public ILocalization Localization => _localizationScope.Localization;

        public Transform HotAirBalloonAnchor => _hotAirBalloonAnchor;
        
        public Transform HotAirBalloonFrom => _hotAirBalloonFrom;
        public Transform HotAirBalloonTo => _hotAirBalloonTo;
        public Transform PlayerTo => _playerTo;

        public CameraTransformation CameraFrom => _cameraFrom;
        public CameraTransformation CameraTo => _cameraTo;

        public AnimationCurve CameraMovementCurve => _cameraMovementCurve;
        public AnimationCurve PlayerMovementCurve => _playerMovementCurve;
        public AnimationCurve PlayerJumpCurve => _cameraJumpCurve;
        public AnimationCurve HotAirBalloonMovementCurve => _hotAirBalloonMovementCurve;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            HotAirBalloon.gameObject.SetActive(false);
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new PrepareState(this),
                new ApproachGroundState(this),
                new JumpOnPedestalState(this),
                new ShowSpeechBubbleState(this)
            };
        }
    }
}