using DopeElections.HotAirBalloon;
using DopeElections.Planets;
using DopeElections.Users;
using UnityEngine;

namespace DopeElections.ScriptedSequences.JourneyToPlanet
{
    public class JourneyToPlanetCinematicController : ScriptedSequenceController
    {
        [Header("Prefab References")] [SerializeField]
        private Transform _environmentAnchor = null;

        [Header("Sequence Configuration")] [Header("Flyby")] [SerializeField]
        private float _flybyTime = 3f;

        [SerializeField] private Transform _flybyFromAnchor = null;

        [SerializeField] private Transform _flybyToAnchor = null;
        [SerializeField] private AnimationCurve _flybyPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _flybyRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _flybyScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Approach")] [SerializeField] private float _approachTime = 2f;
        [SerializeField] private Transform _approachFromAnchor = null;
        [SerializeField] private Transform _approachToAnchor = null;
        [SerializeField] private AnimationCurve _approachPositionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _approachRotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _approachScaleCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Scene References")] [SerializeField]
        private PlayerController _playerController = null;

        [SerializeField] private HotAirBalloonController _hotAirBalloon = null;
        [SerializeField] private PlanetController _planet = null;
        [SerializeField] private Transform _planetAnchor = null;

        internal PlayerController Player => _playerController;
        internal HotAirBalloonController HotAirBalloon => _hotAirBalloon;
        internal PlanetController Planet => _planet;

        internal Transform EnvironmentAnchor => _environmentAnchor;
        internal Transform PlanetAnchor => _planetAnchor;

        internal Transform FlybyFromAnchor => _flybyFromAnchor;
        internal Transform FlybyToAnchor => _flybyToAnchor;
        internal AnimationCurve FlybyPositionCurve => _flybyPositionCurve;
        internal AnimationCurve FlybyRotationCurve => _flybyRotationCurve;
        internal AnimationCurve FlybyScaleCurve => _flybyScaleCurve;

        internal Transform ApproachFromAnchor => _approachFromAnchor;
        internal Transform ApproachToAnchor => _approachToAnchor;
        internal AnimationCurve ApproachPositionCurve => _approachPositionCurve;
        internal AnimationCurve ApproachRotationCurve => _approachRotationCurve;
        internal AnimationCurve ApproachScaleCurve => _approachScaleCurve;

        public bool FlybySkipped { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _planet.HideImmediate();
        }

        protected override void OnPlay(ScriptedSequenceState[] parts)
        {
            base.OnPlay(parts);
            FlybySkipped = false;
            gameObject.SetActive(true);
        }

        protected override void OnStop()
        {
            base.OnStop();
            gameObject.SetActive(false);
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new PrepareCinematicState(this),
                new FlybyState(this, _flybyTime),
                new ApproachPlanetState(this, _approachTime),
            };
        }
    }
}