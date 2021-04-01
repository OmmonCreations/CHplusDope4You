using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Placeholders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Progress
{
    public class LeaderboardEntryController : MonoBehaviour
    {
        public delegate void SelectEvent();

        public event SelectEvent Selected = delegate { };

        [SerializeField] private RectTransform _rectTransform = null;
        [SerializeField] private Button _candidateButton = null;
        [SerializeField] private Image _backgroundImage = null;
        [SerializeField] private Image _portraitImage = null;
        [SerializeField] private TMP_Text _rankText = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private TMP_Text _matchText = null;
        [SerializeField] private LikeDislikeController _likeDislikeController = null;

        private Candidate _candidate;
        private int _rank;
        private float _position;

        public int Index { get; set; }

        public int Rank
        {
            get => _rank;
            set => ApplyRank(value);
        }

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public float Position
        {
            get => _position;
            set => ApplyPosition(value);
        }

        public void Initialize()
        {
            _candidateButton.onClick.AddListener(() => Selected());
            
            DopeElectionsApp.Instance.User.CandidateConfigurationChanged += OnCandidateChanged;
        }

        private void OnDestroy()
        {
            DopeElectionsApp.Instance.User.CandidateConfigurationChanged -= OnCandidateChanged;
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;
            _nameText.text = _candidate.firstName + " " + _candidate.lastName;
            _matchText.text = _candidate.matchString+" %";
            _backgroundImage.color = candidate.GetPartyColor();
            WebSprite.LoadSquare(candidate.urlImage).Assign(s =>
            {
                if (_portraitImage) _portraitImage.sprite = s;
            });

            _likeDislikeController.Candidate = candidate;
        }

        private void ApplyRank(int rank)
        {
            _rank = rank;
            _rankText.text = rank.ToString();
        }

        private void ApplyPosition(float position)
        {
            _position = position;
            _rectTransform.anchoredPosition = new Vector2(0, -position);
        }

        private void OnCandidateChanged(Candidate candidate)
        {
            if(candidate==Candidate) ApplyCandidate(candidate);
        }
    }
}