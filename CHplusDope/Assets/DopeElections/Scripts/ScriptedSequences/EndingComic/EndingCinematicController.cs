using System.Linq;
using DopeElections.Parliaments;
using DopeElections.ScriptedSequences.EndingComic.States;
using DopeElections.Users;
using UnityEngine;
using UnityEngine.Playables;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class EndingCinematicController : CinematicController
    {
        internal delegate void CompleteEvent();

        internal event CompleteEvent Completed = delegate { };

        [SerializeField] private GameObject _cinematicEnvironment = null;
        [SerializeField] private PlayableDirector _cinematicDirector = null;
        [SerializeField] private PlayableDirector _creditsDirector = null;
        [SerializeField] private GameObject _endingComicEnvironment = null;
        [SerializeField] private GameObject _partySequenceEnvironment = null;
        [SerializeField] private ParliamentController _parliament = null;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private PartyCrowdController _backgroundCrowd = null;
        [SerializeField] private PartyCrowdController _winnerCrowd = null;

        internal GameObject Environment => _cinematicEnvironment;
        internal PlayableDirector CinematicDirector => _cinematicDirector;
        internal PlayableDirector CreditsDirector => _creditsDirector;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Environment.SetActive(false);
            CinematicDirector.stopped += TriggerComplete;
            _endingComicEnvironment.SetActive(false);
            _partySequenceEnvironment.SetActive(false);
            _backgroundCrowd.Initialize();
            _winnerCrowd.Initialize();
        }

        protected override void OnPlay(ScriptedSequenceState[] parts)
        {
            _partySequenceEnvironment.SetActive(true);
            _endingComicEnvironment.SetActive(true);
            _playerController.ApplyUserConfiguration();
            _playerController.PlayCelebrationAnimation(0, false);
            _parliament.Locked = false;
            PlaceCandidates();
            BlackMask.BlockInteractions(false);
            BlackMask.FadeToClear();
            base.OnPlay(parts);
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();
            CreditsDirector.Play();
        }

        private void PlaceCandidates()
        {
            var user = DopeElectionsApp.Instance.User;
            var candidates = user.GetRegionalCandidates().ToArray();
            foreach (var c in candidates) c.RecalculateMatch();
            var winners = candidates.OrderByDescending(c => c.match).Take(_winnerCrowd.Count).ToArray();
            _winnerCrowd.PlaceCandidates(winners);
            _backgroundCrowd.PlaceCandidates(winners.Length < candidates.Length
                ? candidates.Except(winners).ToArray()
                : candidates);
        }

        private void TriggerComplete(PlayableDirector director)
        {
            Completed();
        }

        public override void Skip()
        {
            CinematicDirector.time = CinematicDirector.duration - 0.1f;
        }

        protected override ScriptedSequenceState[] GetParts()
        {
            return new ScriptedSequenceState[]
            {
                new PlayCinematicState(this)
            };
        }
    }
}