using System;
using DopeElections.Answer;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class LikeDislikeController : MonoBehaviour
    {
        private static readonly int LikedProperty = Animator.StringToHash("liked");
        private static readonly int DislikedProperty = Animator.StringToHash("disliked");

        [SerializeField] private Toggle _likeToggle = null;
        [SerializeField] private Toggle _dislikeToggle = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;

        private Candidate _candidate;
        private bool _liked;
        private bool _disliked;

        private void Awake()
        {
            _likeToggle.onValueChanged.AddListener(Like);
            _dislikeToggle.onValueChanged.AddListener(Dislike);
        }

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set
            {
                _canvasGroup.interactable = value;
                _canvasGroup.blocksRaycasts = value;
            }
        }

        private void OnEnable()
        {
            if (Interactable) ApplyAnimationImmediate();
            else
            {
                _animator.enabled = false;
                UpdateStateImmediate();
            }

            HookEvents();
        }

        private void OnDisable()
        {
            ReleaseHooks();
        }

        private void HookEvents()
        {
            var user = DopeElectionsApp.Instance.User;
            if (user != null)
            {
                user.CandidateConfigurationChanged += OnCandidateChanged;
            }
        }

        private void ReleaseHooks()
        {
            var user = DopeElectionsApp.Instance.User;
            if (user != null)
            {
                user.CandidateConfigurationChanged -= OnCandidateChanged;
            }
        }

        private void OnCandidateChanged(Candidate candidate)
        {
            if (candidate == Candidate)
            {
                ApplyCandidate(candidate);
            }
        }

        private void ApplyCandidate(Candidate candidate)
        {
            _candidate = candidate;

            var user = DopeElectionsApp.Instance.User;
            var liked = candidate != null && user.LikedCandidates.Contains(candidate.id);
            var disliked = candidate != null && user.DislikedCandidates.Contains(candidate.id);

            _likeToggle.SetIsOnWithoutNotify(liked);
            _dislikeToggle.SetIsOnWithoutNotify(disliked);

            _likeToggle.gameObject.SetActive(!disliked && (liked || Interactable));
            _dislikeToggle.gameObject.SetActive(!liked && (disliked || Interactable));

            _likeToggle.interactable = Interactable && !disliked;
            _dislikeToggle.interactable = Interactable && !liked;

            _liked = liked;
            _disliked = disliked;

            if (Interactable) ApplyAnimationImmediate();
            else UpdateStateImmediate();
        }

        private void ApplyAnimationImmediate()
        {
            if (!Interactable || !gameObject.activeInHierarchy) return;
            var animation = _liked ? "liked" : _disliked ? "disliked" : "idle";
            _animator.SetBool(LikedProperty, _liked);
            _animator.SetBool(DislikedProperty, _disliked);
            _animator.Play(animation);
        }

        private void Like(bool like)
        {
            if (Candidate == null) return;
            var user = DopeElectionsApp.Instance.User;
            user.LikeCandidate(Candidate, like);
            _dislikeToggle.interactable = !like && Interactable;
            if (like)
            {
                _dislikeToggle.SetIsOnWithoutNotify(false);
            }

            _liked = like;
            if (Interactable) _animator.SetBool(LikedProperty, like);
            else UpdateStateImmediate();
        }

        private void Dislike(bool dislike)
        {
            if (Candidate == null) return;
            var user = DopeElectionsApp.Instance.User;
            user.DislikeCandidate(Candidate, dislike);
            _likeToggle.interactable = !dislike && Interactable;
            if (dislike)
            {
                _likeToggle.SetIsOnWithoutNotify(false);
            }

            _disliked = dislike;
            if (Interactable) _animator.SetBool(DislikedProperty, dislike);
            else UpdateStateImmediate();
        }

        private void UpdateStateImmediate()
        {
            _likeToggle.gameObject.SetActive(_liked);
            _dislikeToggle.gameObject.SetActive(_disliked);
        }
    }
}