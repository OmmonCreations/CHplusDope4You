using System.Linq;
using BlackMasks;
using DopeElections.Candidates;
using DopeElections.Localizations;
using DopeElections.Races;
using DopeElections.Sounds;
using Essentials;
using FMODSoundInterface;
using Localizator;
using StateMachines;
using UnityEngine;

namespace DopeElections.RaceResults.Celebration
{
    public class CelebrationView : RaceResultView
    {
        public override NamespacedKey Id => RaceResultViewId.Celebration;

        [SerializeField] private BlackMask _whiteMask = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private Transform _environmentAnchor = null;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private LocalizedText _titleText = null;
        [SerializeField] private RaceProgressDisplayController _progressDisplay = null;
        [SerializeField] private CandidateController[] _celebratingCandidates = null;
        [SerializeField] private ParticleSystem _celebrationParticles = null;
        [SerializeField] private AnimationCurve _candidateJumpArc = null;

        private StateMachine StateMachine => _stateMachine;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _camera.enabled = false;
            _titleText.key = LKey.Views.Celebration.CategoryComplete;
            _environmentAnchor.gameObject.SetActive(false);
            _whiteMask.Alpha = 0;
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            _environmentAnchor.gameObject.SetActive(true);

            var categoryRace = Context.Race as QuestionMarathon;
            if (categoryRace != null)
            {
                _progressDisplay.Initialize(categoryRace.Questions.Select(q => true).ToList(), null);
            }
            else
            {
                _progressDisplay.gameObject.SetActive(false);
            }

            Views.BlackMask.Alpha = 0;
            Views.BlackMask.BlockInteractions(false);
            _whiteMask.FadeToClear().Then(TriggerCelebration);
        }

        private void TriggerCelebration()
        {
            var celebratingCandidates = _celebratingCandidates;
            var bestCandidates = Context.Race.Winners.Take(celebratingCandidates.Length).ToList();
            for (var i = 0; i < bestCandidates.Count; i++)
            {
                celebratingCandidates[i].LOD = 2;
                celebratingCandidates[i].Candidate = bestCandidates[i].Candidate;
                celebratingCandidates[i].gameObject.SetActive(true);
                celebratingCandidates[i].Jump(5, 2.5f - i * 0.3f, _candidateJumpArc);
            }

            for (var i = bestCandidates.Count; i < celebratingCandidates.Length; i++)
            {
                celebratingCandidates[i].gameObject.SetActive(false);
            }

            _celebrationParticles.Play();
            _camera.enabled = true;
                
            SoundController.Play(Sound.Sfx.Special.RaceComplete);

            StateMachine.State = new DelayedActionState(Continue, 3f);
        }

        protected override void OnClose()
        {
            base.OnClose();
            _environmentAnchor.gameObject.SetActive(false);
            _camera.enabled = false;
            _whiteMask.BlockInteractions(false);
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void Continue()
        {
            Views.ReviewView.Open(Context, true);
        }
    }
}