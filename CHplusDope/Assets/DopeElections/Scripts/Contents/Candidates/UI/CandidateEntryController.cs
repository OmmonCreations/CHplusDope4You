using System.Linq;
using DopeElections.Answer;
using DopeElections.CandidateParties;
using DopeElections.Users;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class CandidateEntryController : MonoBehaviour
    {
        [SerializeField] private Image _frameImage = null;
        [SerializeField] private Image _frameAccentImage = null;
        [SerializeField] private Image _wearingHatImage = null;
        [SerializeField] private Image _portraitImage = null;
        [SerializeField] private TMP_Text _matchText = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Sprite _placeholderSprite = null;
        [Header("Sprites")] [SerializeField] private Sprite _frameEmpty = null;
        [SerializeField] private Sprite _frameDislike = null;
        [SerializeField] private Sprite _frameDislikeTeam = null;
        [SerializeField] private Sprite _frameLike = null;
        [SerializeField] private Sprite _frameLikeTeam = null;
        [SerializeField] private Sprite _frameNeutral = null;
        [SerializeField] private Sprite _frameNeutralTeam = null;

        private Candidate _candidate;

        private ActiveUser User { get; set; }

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        private void Start()
        {
            User = DopeElectionsApp.Instance.User;
            HookEvents();
        }

        private void OnDestroy()
        {
            ReleaseHooks();
        }

        private void HookEvents()
        {
            if (User != null)
            {
                User.CandidateConfigurationChanged += OnCandidateChanged;
            }
        }

        private void ReleaseHooks()
        {
            if (User != null)
            {
                User.CandidateConfigurationChanged -= OnCandidateChanged;
            }
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public void UpdateFrame()
        {
            if (Candidate == null)
            {
                _frameAccentImage.enabled = false;
                _frameImage.sprite = _frameEmpty;
                _wearingHatImage.enabled = false;
                return;
            }

            var user = DopeElectionsApp.Instance.User;
            var party = DopeElectionsApp.Instance.Assets.GetAsset<Party>(p =>
                p.id == Candidate.partyId);
            _frameAccentImage.enabled = party != null;
            _frameAccentImage.color = party != null ? party.GetColor() : default;

            var candidateUserData = user.GetCandidateUserData(_candidate.id);
            var userTeam = user.GetActiveList();
            var like = user.LikedCandidates.Contains(Candidate.id);
            var dislike = user.DislikedCandidates.Contains(Candidate.id);
            var team = userTeam != null && userTeam.candidates.Any(e => e.id == Candidate.id);
            Sprite sprite;
            if (team)
            {
                sprite = like
                    ? _frameLikeTeam
                    : dislike
                        ? _frameDislikeTeam
                        : _frameNeutralTeam;
            }
            else
            {
                sprite = like
                    ? _frameLike
                    : dislike
                        ? _frameDislike
                        : _frameNeutral;
            }

            _frameImage.sprite = sprite;
            _wearingHatImage.enabled = candidateUserData!=null && candidateUserData.Hat != default;
        }

        private void OnCandidateChanged(Candidate candidate)
        {
            if (candidate != Candidate) return;
            UpdateFrame();
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;
            // _frameImage.color = candidate.partyColor;
            _matchText.enabled = candidate != null;
            _nameText.enabled = candidate != null;
            if (candidate != null)
            {
                _matchText.text = "id:" + candidate.id;
                _nameText.text = candidate.lastName;
                _portraitImage.enabled = true;
                candidate.GetIcon(_placeholderSprite).Assign(s =>
                {
                    if (_portraitImage && candidate == _candidate) _portraitImage.sprite = s;
                });
            }
            else
            {
                _portraitImage.enabled = false;
                _matchText.text = "";
                _nameText.text = "";
            }

            UpdateFrame();
        }
    }
}