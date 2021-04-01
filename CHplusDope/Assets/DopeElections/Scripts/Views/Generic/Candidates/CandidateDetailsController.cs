using System;
using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.CandidateParties;
using DopeElections.Localizations;
using DopeElections.Placeholders;
using DopeElections.SmartSpiders;
using Localizator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class CandidateDetailsController : MonoBehaviour
    {
        [SerializeField] private CandidateSubviews _subviews = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _portraitImage = null;
        [SerializeField] private Sprite _placeholderSprite = null;
        [SerializeField] private SmartSpiderController _smartSpider = null;
        [SerializeField] private SlidablePanelController _mainPanelController = null;
        [SerializeField] private Color _noPartyColor = Color.gray;
        [SerializeField] private Graphic[] _accentGraphics = null;
        [SerializeField] private LikeDislikeController _likeDislikeController = null;
        [SerializeField] private ElectionInfoController _electionInfoController = null;

        [SerializeField] private LocalizedText _matchText = null;
        [SerializeField] private LocalizedText _compareText = null;
        [SerializeField] private LocalizedText _ageText = null;
        [SerializeField] private LocalizedText _jobText = null;
        [SerializeField] private LocalizedText _locationText = null;
        [SerializeField] private LocalizedText _readMoreText = null;

        private Candidate _candidate;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public SmartSpiderController SmartSpiderController => _smartSpider;
        public SlidablePanelController MainPanelController => _mainPanelController;

        private void Awake()
        {
            _matchText.key = LKey.Components.Candidate.Match.Label;
            _compareText.key = LKey.Views.Candidate.Compare;
            _readMoreText.key = LKey.Views.Candidate.ReadMore;

            MainPanelController.Appears += HideSmartSpiderImmediate;
            MainPanelController.Appeared += ShowSmartSpider;
        }

        private void ApplyCandidate(Candidate candidate)
        {
            candidate.RecalculateMatch();
            candidate.RecalculateSmartSpider();
            _candidate = candidate;
            _nameText.text = candidate.fullName;
            _matchText.SetVariable("value", candidate.matchString);
            _ageText.key = new LocalizationKey
            {
                fallback = candidate.birthYear != 0 ? (DateTime.Now.Year - candidate.birthYear).ToString() : "-"
            };
            _jobText.key = new LocalizationKey
            {
                fallback = !string.IsNullOrWhiteSpace(candidate.occupation) ? candidate.occupation : "-"
            };
            _locationText.key = new LocalizationKey
            {
                fallback = !string.IsNullOrWhiteSpace(candidate.city) ? candidate.city : "-"
            };

            _smartSpider.Initialize();
            _smartSpider.ClearAllValue();
            _smartSpider.ShowCandidate(candidate);
            _smartSpider.ShowUser(true);

            _likeDislikeController.Candidate = candidate;
            _electionInfoController.Candidate = candidate;

            WebSprite.Load(candidate.urlImage, _placeholderSprite).Assign(s =>
            {
                if (_portraitImage) _portraitImage.sprite = s;
            });

            var party = candidate.GetParty();
            var accentColor = party != null ? party.GetColor() : _noPartyColor;
            ApplyAccentColor(accentColor);

            _subviews.Candidate = candidate;
            _subviews.ProfileView.Open();

            //Debug.Log("candidate response : " + candidate.responses[0]);
        }

        private void ApplyAccentColor(Color color)
        {
            foreach (var g in _accentGraphics) g.color = color;
        }

        private void ShowSmartSpider()
        {
            _smartSpider.Show();
        }

        private void HideSmartSpiderImmediate(float t)
        {
            _smartSpider.HideImmediate();
        }
    }
}