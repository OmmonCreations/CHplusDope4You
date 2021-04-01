using System;
using System.Linq;
using DopeElections.Candidates;
using DopeElections.PoliticalCharacters;
using DopeElections.Users;
using Localizator;
using SpeechBubbles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.ScriptedSequences.Splash
{
    public class SplashCinematicController : ScriptedSequenceController, IPoliticalCharacterEnvironment
    {
        [Header("Scene References")] [SerializeField]
        private LocalizationScope _localizationScope = null;

        [SerializeField] private SpeechBubbleLayer _speechBubbleLayer = null;

        [Header("Candidates")] [SerializeField]
        private CandidateController _candidateTemplate = null;

        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private Transform _candidatesAnchor = null;
        [SerializeField] private Transform _start = null;
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _candidatePositionRandomization = 5;
        [SerializeField] private float _candidateSpeed = 10;
        [SerializeField] private float _candidateAnimationTime = 5;
        [SerializeField] private CandidateData[] _candidates = null;

        [Header("Logo Animation")] [SerializeField]
        private CanvasGroup _logoGroup = null;

        [SerializeField] private RectTransform _logoTransform = null;
        [SerializeField] private CanvasGroup _subtitleGroup = null;
        [SerializeField] private RectTransform _subtitleTransform = null;
        [SerializeField] private CanvasGroup _playButtonGroup = null;
        [SerializeField] private RectTransform _playButtonTransform = null;
        [SerializeField] private float _logoAnimationTime = 5;
        [SerializeField] private AnimationCurve _logoSizeCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private AnimationCurve _logoOpacityCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _subtitleSizeCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private AnimationCurve _subtitleOpacityCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _playButtonSizeCurve = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField] private AnimationCurve _playButtonOpacityCurve = AnimationCurve.Linear(0, 0, 1, 1);

        public ISpeechBubbleSource SpeechBubbleSource => _speechBubbleLayer;
        public ILocalization Localization => _localizationScope.Localization;
        public CanvasGroup LogoGroup => _logoGroup;
        public RectTransform LogoTransform => _logoTransform;
        public CanvasGroup SubtitleGroup => _subtitleGroup;
        public RectTransform SubtitleTransform => _subtitleTransform;
        public CanvasGroup PlayButtonGroup => _playButtonGroup;
        public RectTransform PlayButtonTransform => _playButtonTransform;
        public AnimationCurve LogoSizeCurve => _logoSizeCurve;
        public AnimationCurve LogoOpacityCurve => _logoOpacityCurve;
        public AnimationCurve SubtitleSizeCurve => _subtitleSizeCurve;
        public AnimationCurve SubtitleOpacityCurve => _subtitleOpacityCurve;
        public AnimationCurve PlayButtonSizeCurve => _playButtonSizeCurve;
        public AnimationCurve PlayButtonOpacityCurve => _playButtonOpacityCurve;

        public PlayerController PlayerController => _playerController;
        
        public CandidateController[] SpawnCandidates(Vector3 position, float randomization)
        {
            return _candidates.Select(c => SpawnCandidate(c, position, randomization)).ToArray();
        }

        private CandidateController SpawnCandidate(CandidateData data, Vector3 position, float randomization)
        {
            var instanceObject = Instantiate(_candidateTemplate.gameObject, _candidatesAnchor, false);
            var instance = instanceObject.GetComponent<CandidateController>();
            instance.Portrait = data.sprite.texture;
            instance.BodyColor = data.color;
            instance.Initialize(this);
            var transform = instanceObject.transform;
            transform.position = position + new Vector3(
                (Random.value * 2 - 1) * randomization,
                0,
                (Random.value * 2 - 1) * randomization);
            return instance;
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new PrepareCinematicState(this),
                new CandidatesRunState(this, _start.position, _target.position, _candidateSpeed,
                    _candidatePositionRandomization, _candidateAnimationTime),
                new TitleAnimationState(this, _logoAnimationTime)
            };
        }

        [Serializable]
        public class CandidateData
        {
            public Sprite sprite;
            public Color color = Color.white;
        }
    }
}