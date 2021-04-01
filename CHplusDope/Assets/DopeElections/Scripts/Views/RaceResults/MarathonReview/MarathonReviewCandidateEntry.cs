using AnimatedObjects;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Placeholders;
using DopeElections.Races;
using Localizator;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.RaceResults
{
    public class MarathonReviewCandidateEntry : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private LocalizedText _matchText = null;
        [SerializeField] private Image _candidateImage = null;
        [SerializeField] private Image _candidateFrame = null;
        [SerializeField] private Sprite _placeholderSprite = null;
        [SerializeField] private LikeDislikeController _likeDislikeController = null;
        [SerializeField] private ToggleableObjectController _animationController = null;

        private RaceCandidate _candidate;
        public UnityEvent onClick => _button.onClick;

        public LocalizedText MatchText => _matchText;

        public ToggleableObjectController AnimationController => _animationController;

        public RaceCandidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void ApplyCandidate(RaceCandidate candidate)
        {
            _candidate = candidate;
            _nameText.text = candidate != null ? candidate.firstName + " " + candidate.lastName : "";

            var matchValue = (candidate != null ? Mathf.RoundToInt(candidate.CategoryMatch * 100) : 0);
            var portrait = candidate != null ? candidate.Candidate.urlImage : null;
            MatchText.SetVariable("value", matchValue.ToString());

            if (portrait != null)
            {
                WebSprite.LoadSquare(portrait, _placeholderSprite).Assign(s =>
                {
                    if (_candidateImage) _candidateImage.sprite = s;
                    if (_candidateFrame) _candidateFrame.color = _candidate.Candidate.GetPartyColor();
                });
            }
            else
            {
                _candidateImage.sprite = _placeholderSprite;
            }

            _likeDislikeController.Candidate = candidate != null ? candidate.Candidate : null;
        }
    }
}