using System.Collections.Generic;
using System.Linq;
using AnimatedObjects;
using DopeElections.Localizations;
using DopeElections.Races;
using DopeElections.Sounds;
using Essentials;
using FMODSoundInterface;
using Localizator;
using UnityEngine;
using UnityEngine.UI;
using Views;

namespace DopeElections.RaceResults
{
    public class MarathonReviewView : RaceResultView, IView<RaceContext, bool>
    {
        public override NamespacedKey Id => RaceResultViewId.MarathonReview;

        [SerializeField] private LocalizedText _challengeCompleteText = null;
        [SerializeField] private LocalizedText _raceLabelText = null;
        [SerializeField] private LocalizedText _smartSpiderText = null;
        [SerializeField] private LocalizedText _resultTypeText = null;
        [SerializeField] private LocalizedText _hugeTieText = null;
        [SerializeField] private LocalizedText _continueText = null;
        [SerializeField] private LocalizedText _repeatText = null;
        [SerializeField] private Image _raceIconImage = null;
        [SerializeField] private RectTransform _candidatesArea = null;
        [SerializeField] private LayoutElement _candidatesAreaElement = null;
        [SerializeField] private MarathonReviewCandidateEntry _templateSmall = null;
        [SerializeField] private MarathonReviewCandidateEntry _templateLarge = null;
        [SerializeField] private Button _continueButton = null;
        [SerializeField] private Button _repeatButton = null;
        [SerializeField] private float _smallHeight = 146;
        [SerializeField] private float _largeHeight = 400;
        [SerializeField] private VerticalLayoutGroup _mainGroup = null;
        [SerializeField] private float _smallSpacing = 70;
        [SerializeField] private float _largeSpacing = 50;
        [SerializeField] private Vector2Int _smallMargins = Vector2Int.zero;
        [SerializeField] private Vector2Int _largeMargins = Vector2Int.zero;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private PoppablePanelController _repeatButtonAnimationController = null;
        [SerializeField] private PoppablePanelController _confirmButtonAnimationController = null;

        private readonly List<MarathonReviewCandidateEntry> _entries = new List<MarathonReviewCandidateEntry>();
        private bool PlayAnimation { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _challengeCompleteText.key = LKey.Views.RaceReview.ChallengeComplete;
            _hugeTieText.key = LKey.Views.RaceReview.HugeTieText;
            _smartSpiderText.key = LKey.Views.RaceReview.SmartSpider;
            _continueText.key = LKey.Views.RaceReview.Continue;
            _repeatText.key = LKey.Views.RaceReview.Repeat;

            _templateSmall.gameObject.SetActive(false);
            _templateLarge.gameObject.SetActive(false);

            _continueButton.onClick.AddListener(Continue);
            _repeatButton.onClick.AddListener(Repeat);
        }

        public void Open(RaceContext context, bool playAnimation)
        {
            PlayAnimation = playAnimation;
            base.Open(context);
        }

        public new void Open(RaceContext context)
        {
            PlayAnimation = false;
            base.Open(context);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateCandidates();
            _raceLabelText.key = Context.Race.Label;
            _raceIconImage.sprite = Context.Race.IconWhite;

            Views.BlackMask.BlockInteractions(false);
            if (Views.BlackMask.Alpha > 0) Views.BlackMask.FadeToClear();

            if (PlayAnimation)
            {
                var isTie = _entries.Count > 3;
                _animator.Play(isTie ? "animation-tie" : "animation-normal");
                var entriesDelay = isTie ? 1 : 0.75f;
                var buttonsDelay = entriesDelay + _entries.Count * 0.2f + 0.5f;
                var indices = _entries.Select((e, i) => i).ToList();
                if (indices.Count == 3)
                {
                    indices[1] = 0;
                    indices[0] = 1;
                }

                for (var i = 0; i < _entries.Count; i++)
                {
                    var index = indices[i];
                    var e = _entries[index];
                    e.AnimationController.Show(entriesDelay + i * 0.2f);
                }

                _repeatButtonAnimationController.Show(buttonsDelay);
                _confirmButtonAnimationController.Show(buttonsDelay + 0.1f);
            }
            else
            {
                _confirmButtonAnimationController.ShowImmediate();
                _repeatButtonAnimationController.ShowImmediate();
            }
        }

        protected override void OnClose()
        {
            PlayAnimation = false;
            base.OnClose();
        }

        private void UpdateCandidates()
        {
            ClearEntries();

            var race = Context.Race;

            var winners = race.Winners;

            MarathonReviewCandidateEntry prefab;
            LocalizationKey resultTypeLabel;
            bool isLarge;
            bool showHugeTieText;
            int candidateCount;
            float height;
            if (winners.Length > QuestionMarathon.MaxWinners)
            {
                prefab = _templateSmall;
                candidateCount = QuestionMarathon.MaxTiedWinners;
                resultTypeLabel = LKey.Views.RaceReview.HugeTie;
                isLarge = false;
                showHugeTieText = true;
                height = _smallHeight;
            }
            else
            {
                prefab = _templateLarge;
                candidateCount = QuestionMarathon.MaxWinners;
                resultTypeLabel = LKey.Views.RaceReview.TopChampions;
                isLarge = true;
                showHugeTieText = false;
                height = _largeHeight;

                if (winners.Length == 3)
                {
                    //put first place in center
                    var firstPlace = winners[0];
                    var secondPlace = winners[1];
                    winners[0] = secondPlace;
                    winners[1] = firstPlace;
                }
            }

            _resultTypeText.key = resultTypeLabel;
            _hugeTieText.gameObject.SetActive(showHugeTieText);
            _candidatesAreaElement.preferredHeight = height;
            _candidatesAreaElement.minHeight = height;
            _entries.AddRange(CreateEntries(prefab, winners.Take(candidateCount)));

            _mainGroup.spacing = isLarge ? _largeSpacing : _smallSpacing;
            _mainGroup.padding.top = isLarge ? _largeMargins.x : _smallMargins.x;
            _mainGroup.padding.bottom = isLarge ? _largeMargins.y : _smallMargins.y;
        }

        private void OpenCandidateDetails(RaceCandidate candidate)
        {
            DopeElectionsApp.Instance.Views.CandidateView.Open(candidate.Candidate);
        }

        private void Continue()
        {
            DopeElectionsApp.Instance.User.PreviousSmartSpider = DopeElectionsApp.Instance.User.SmartSpider;
            Views.Continue(Context);
        }

        private void Repeat()
        {
            Views.Repeat(Context);
        }

        private void ClearEntries()
        {
            foreach (var entry in _entries)
            {
                entry.Remove();
            }

            _entries.Clear();
        }

        private IEnumerable<MarathonReviewCandidateEntry> CreateEntries(MarathonReviewCandidateEntry prefab,
            IEnumerable<RaceCandidate> candidates)
        {
            return candidates.Select(c => CreateEntry(prefab, c));
        }

        private MarathonReviewCandidateEntry CreateEntry(MarathonReviewCandidateEntry prefab, RaceCandidate candidate)
        {
            var instanceObject = Instantiate(prefab.gameObject, _candidatesArea, false);
            var instance = instanceObject.GetComponent<MarathonReviewCandidateEntry>();
            instance.onClick.AddListener(() => OpenCandidateDetails(candidate));
            instance.MatchText.key = LKey.Views.RaceReview.CategoryMatch;
            instance.Candidate = candidate;
            instanceObject.SetActive(true);
            return instance;
        }
    }
}