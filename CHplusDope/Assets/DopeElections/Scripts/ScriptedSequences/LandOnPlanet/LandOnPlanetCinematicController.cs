using CameraSystems;
using DopeElections.HotAirBalloon;
using DopeElections.Planets;
using DopeElections.ScriptedSequences.IntroComic;
using DopeElections.ScriptedSequences.LandOnPlanet.States;
using DopeElections.ScriptedSequences.PlanetChase;
using DopeElections.Users;
using UnityEngine;
using PrepareState = DopeElections.ScriptedSequences.LandOnPlanet.States.PrepareState;

namespace DopeElections.ScriptedSequences.LandOnPlanet
{
    public class LandOnPlanetCinematicController : ScriptedSequenceController
    {
        [Header("Land")] [SerializeField] private float _landTime = 4f;
        [SerializeField] private Transform _envirnomentAnchor = null;
        [SerializeField] private Transform _landFromAnchor = null;
        [SerializeField] private Transform _landToAnchor = null;
        [SerializeField] private AnimationCurve _landPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _landRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _landScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _cameraTransitionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private CameraTransformation _cameraTransformation = default;
        [SerializeField] private IntroComicSequenceController _comicSequenceController = null;
        [SerializeField] private PlanetChaseCinematicController _planetChaseController = null;

        [Header("Transition to 3d")]
        [SerializeField] private CameraTransformation _cameraFadeBackTransformationA = default;
        [SerializeField] private CameraTransformation _cameraFadeBackTransformationB = default;

        [Header("Scene References")] [SerializeField]
        private PlayerController _playerController = null;

        [SerializeField] private CameraSystem _cameraSystem = null;

        [SerializeField] private HotAirBalloonController _hotAirBalloon = null;
        [SerializeField] private PlanetController _planet = null;

        public Transform EnvironmentAnchor => _envirnomentAnchor;
        internal PlayerController Player => _playerController;
        internal HotAirBalloonController HotAirBalloon => _hotAirBalloon;
        internal PlanetController Planet => _planet;
        public CameraSystem CameraSystem => _cameraSystem;
        public CameraTransformation CameraTransformation => _cameraTransformation;
        public AnimationCurve CameraTransitionCurve => _cameraTransitionCurve;
        public IntroComicSequenceController ComicSequence => _comicSequenceController;
        public PlanetChaseCinematicController PlanetChase => _planetChaseController;

        internal Transform LandFromAnchor => _landFromAnchor;
        internal Transform LandToAnchor => _landToAnchor;
        internal AnimationCurve LandPositionCurve => _landPositionCurve;
        internal AnimationCurve LandRotationCurve => _landRotationCurve;
        internal AnimationCurve LandScaleCurve => _landScaleCurve;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _planet.HideImmediate();
            ComicSequence.SecondPartCompleted += PlanetChase.Prepare;
            ComicSequence.TransitionTo3dStarted += OnTransitionTo3dStarted;
            ComicSequence.Initialize();
            PlanetChase.Initialize();
        }

        protected override void OnPlay(ScriptedSequenceState[] parts)
        {
            base.OnPlay(parts);
            gameObject.SetActive(true);
        }

        protected override void OnStop()
        {
            base.OnStop();
            gameObject.SetActive(false);
        }

        private void OnTransitionTo3dStarted()
        {
            const float cameraTransitionTime = (2644 - 2375) / 60f;

            PlanetChase.ApplyPlanetRotation();
            CameraSystem.CurrentTransform = _cameraFadeBackTransformationA;
            CameraSystem.Transition(_cameraFadeBackTransformationB, cameraTransitionTime,
                AnimationCurve.EaseInOut(0, 0, 1, 1));
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new PrepareState(this),
                new LandOnPlanetState(this, _landTime),
                // new PlayComicState(this),
                new WaitForComicState(this),
                new PlanetChaseState(this)
            };
        }
    }
}