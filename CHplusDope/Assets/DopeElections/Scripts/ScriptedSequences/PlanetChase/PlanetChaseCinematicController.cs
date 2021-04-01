using AnimatedObjects;
using CameraSystems;
using DopeElections.Candidates;
using DopeElections.HotAirBalloon;
using DopeElections.Planets;
using DopeElections.Users;
using Essentials;
using Localizator;
using SpeechBubbles;
using UnityEngine;

namespace DopeElections.ScriptedSequences.PlanetChase
{
    public class PlanetChaseCinematicController : ScriptedSequenceController
    {
        [Header("References")]
        [SerializeField] private Transform _environment = null;

        [SerializeField] private CameraSystem _cameraSystem = null;
        [SerializeField] private HotAirBalloonController _hotAirBalloon = null;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private PlanetController _planet = null;
        [SerializeField] private CandidateController[] _candidates = null;
        [SerializeField] private ToggleableObjectController[] _progressSteps = null;
        [SerializeField] private SpeechBubbleLayer _speechBubbleLayer = null;
        [SerializeField] private LocalizationScope _localizationScope = null;
        
        [Header("Anchors")]
        [SerializeField] private Transform _hotAirBalloonPivot = null;
        [SerializeField] private Transform _hotAirBalloonAnchor = null;
        [SerializeField] private Transform _candidatesPivot = null;
        [Header("Reference Points")]
        [SerializeField] private Transform _hotAirBalloonFrom = null;
        [SerializeField] private Transform _hotAirBalloonTo = null;
        [SerializeField] private Transform _candidatesFrom = null;
        [SerializeField] private Transform _candidatesTo = null;
        [SerializeField] private CameraTransformation _cameraFrom = default;
        [SerializeField] private CameraTransformation _cameraTo = default;
        [Header("Animations")] [SerializeField]
        private Vector3 _planetRotation = Vector3.zero;
        [SerializeField] private AnimationCurve _cameraMovementCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _playerMovementCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _candidatesMovementCurve = AnimationCurve.Linear(0,0,1,1);
        [SerializeField] private AnimationCurve _progressStepsAppearCurve = AnimationCurve.Linear(0,0,1,1);

        public Transform Environment => _environment;
        public CameraSystem CameraSystem => _cameraSystem;
        public HotAirBalloonController HotAirBalloon => _hotAirBalloon;
        public PlayerController Player => _playerController;
        internal PlanetController Planet => _planet;
        public CandidateController[] Candidates => _candidates;
        public ToggleableObjectController[] ProgressSteps => _progressSteps;
        public SpeechBubbleLayer SpeechBubbleLayer => _speechBubbleLayer;
        public ILocalization Localization => _localizationScope.Localization;

        public Transform HotAirBalloonPivot => _hotAirBalloonPivot;
        public Transform HotAirBalloonAnchor => _hotAirBalloonAnchor;
        public Transform CandidatesPivot => _candidatesPivot;
        
        public Transform HotAirBalloonFrom => _hotAirBalloonFrom;
        public Transform HotAirBalloonTo => _hotAirBalloonTo;
        
        public Transform CandidatesFrom => _candidatesFrom;
        public Transform CandidatesTo => _candidatesTo;

        public CameraTransformation CameraFrom => _cameraFrom;
        public CameraTransformation CameraTo => _cameraTo;

        public AnimationCurve CameraMovementCurve => _cameraMovementCurve;
        public AnimationCurve PlayerMovementCurve => _playerMovementCurve;
        public AnimationCurve CandidatesMovementCurve => _candidatesMovementCurve;
        public AnimationCurve ProgressStepsCurve => _progressStepsAppearCurve;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _environment.gameObject.SetActive(false);
        }

        public void Prepare()
        {
            Environment.gameObject.SetActive(true);

            Player.transform.SetParent(HotAirBalloon.CharacterAnchor, false);
            Player.transform.Reset();

            HotAirBalloon.transform.SetParent(HotAirBalloonAnchor, false);
            HotAirBalloon.transform.Reset();

            foreach (var step in ProgressSteps) step.HideImmediate();

            CameraSystem.CurrentTransform = CameraFrom;
            
            ApplyPlanetRotation();
        }

        public void ApplyPlanetRotation()
        {
            Planet.AutoRotate = false;
            Planet.MeshTransform.eulerAngles = _planetRotation;
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new PrepareState(this),
                new ShowSpeechBubbleState(this),
                new ChaseAroundPlanetState(this)
            };
        }
    }
}