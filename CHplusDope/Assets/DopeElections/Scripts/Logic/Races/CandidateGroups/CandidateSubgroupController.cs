using System;
using System.Linq;
using DopeElections.Races.RaceTracks;
using DopeElections.Sounds;
using FMODSoundInterface;
using Localizator;
using MobileInputs;
using StateMachines;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DopeElections.Races
{
    public class CandidateSubgroupController : MonoBehaviour, ITapListener
    {
        [SerializeField] private Transform _root = null;
        [SerializeField] private Transform _labelTransform = null;
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private LocalizedText _labelText = null;
        // [SerializeField] private Graphic _labelGraphic = null;
        [SerializeField] private Image _raceIconImage = null;
        [SerializeField] private float _referenceWidth = 1000;
        [SerializeField] private BoxCollider _interactionCollider = null;

        private StateMachine _alphaStateMachine = null;
        private StateMachine _agreementStateMachine = null;
        private CandidateSubgroup _subgroup;
        private float _forwardPosition;
        private float _width;
        private float _length;
        private bool _interactable;

        public Transform Root => _root;
        public CandidateGroupController GroupController { get; private set; }
        public RaceTrack RaceTrack { get; private set; }

        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                if (_interactionCollider) _interactionCollider.enabled = value;
            }
        }

        public float RelativeForwardPosition { get; set; }

        public CandidateSubgroup Subgroup
        {
            get => _subgroup;
            set => ApplyGroup(value);
        }

        public float ForwardPosition
        {
            get => _forwardPosition;
            set => ApplyForwardPosition(value);
        }

        public float Length
        {
            get => _length;
            set => ApplyLength(value);
        }

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        public bool IsVisible => gameObject.activeSelf;

        public void Initialize(CandidateGroupController groupController)
        {
            var raceController = groupController.RaceController;
            GroupController = groupController;
            RaceTrack = raceController.RaceTrack;
            _alphaStateMachine = gameObject.AddComponent<StateMachine>();
            _agreementStateMachine = gameObject.AddComponent<StateMachine>();
            // SetAgreement(0);
        }

        private void Update()
        {
            _alphaStateMachine.Run();
            _agreementStateMachine.Run();
        }

        public void Show() => Show(true);
        public void Hide() => Show(false);

        public void Show(bool show)
        {
            var targetAlpha = show ? 1 : 0;
            var startAlpha = show ? 0 : 1;
            var currentAlpha = Alpha;
            var time = 0.5f;
            var start_t = 1 - (targetAlpha - currentAlpha) / (targetAlpha - startAlpha);
            var transition = new TransitionState(time, startAlpha, targetAlpha, start_t);
            transition.OnTransition += t => Alpha = t;
            transition.OnCompleted += () => Alpha = targetAlpha;
            if (!show) transition.OnCompleted += () => gameObject.SetActive(false);
            _alphaStateMachine.State = transition;
            gameObject.SetActive(true);
        }

        public void ShowImmediate() => ShowImmediate(true);
        public void HideImmediate() => ShowImmediate(false);

        public void ShowImmediate(bool show)
        {
            if (!gameObject) return;
            gameObject.SetActive(show);
            if (show) Alpha = 1;
            else _agreementStateMachine.State = null;
            _alphaStateMachine.State = null;
        }

        private void ApplyForwardPosition(float position)
        {
            _forwardPosition = position;
            var raceTrackPosition = new RaceTrackVector(0.5f, position, RaceTrackVector.AxisType.Percentage,
                RaceTrackVector.AxisType.Distance);
            var width = RaceTrack.Width;

            Root.localPosition = RaceTrack.GetWorldPosition(raceTrackPosition);
            Root.localRotation = Quaternion.identity;
            _labelTransform.localScale = Vector3.one / _referenceWidth * width;
            _width = width;
            ApplyLength(_length);
        }

        private void ApplyLength(float length)
        {
            _length = length;

            var collider = _interactionCollider;
            var labelLength = GroupController.Group.LayoutConfiguration.GroupSpacing;
            var width = _width;
            var height = collider.size.y;

            collider.size = new Vector3(width, height, (length + labelLength));
            collider.center = new Vector3(0, -height / 2, labelLength / 2 - (length + labelLength) / 2);
        }

        private void ApplyGroup(CandidateSubgroup group)
        {
            if (!gameObject) return;
            var oldGroup = _subgroup;
            _subgroup = group;
            if (group == null || group.Type == null)
            {
                HideImmediate();
                return;
            }

            var showLabel = group.ShowLabel;
            _labelTransform.gameObject.SetActive(showLabel);

            if (!showLabel) return;

            _labelText.key = group.Type.Name;
            // _labelGraphic.color = group.Type.Color;
            _raceIconImage.sprite = group.Icon;
            _raceIconImage.enabled = _raceIconImage.sprite;
            var oldAgreementScore = oldGroup != null ? oldGroup.Match.max : 0;
            var newAgreementScore = group != null ? group.Match.max : 0;
            var transition = new TransitionState(0.5f, 0, 1);
            transition.OnTransition += t =>
            {
                SetAgreement(Mathf.SmoothStep(oldAgreementScore, newAgreementScore, t));
            };
            transition.OnCompleted += () => SetAgreement(newAgreementScore);
            _agreementStateMachine.State = transition;
        }

        private void SetAgreement(float agreement)
        {
            var text = Mathf.RoundToInt(agreement * 100) + "%";
            _labelText.key = new LocalizationKey {fallback = text};
        }

        public void OnTap(InputAction.CallbackContext context)
        {
            if (!Interactable) return;
            OpenSubgroupLeaderboard();
            SoundController.Play(Sound.UI.Button.Tap);
        }

        private void OpenSubgroupLeaderboard()
        {
            var subgroup = Subgroup;
            if (subgroup == null) return;
            
            var group = GroupController.Group;
            var maxCategoryMatch = subgroup.Match.max;
            var similarGroups = group.Groups.Where(g => Math.Abs(g.Match.max - maxCategoryMatch) <= 0).ToList();

            var previousGroup = group.Groups.Where(g => g.Match.max < maxCategoryMatch)
                .OrderByDescending(g => g.Match.max).FirstOrDefault();
            var minCategoryMatch = previousGroup != null
                ? previousGroup.Match.max + 0.01f
                : similarGroups.Select(g=>g.Match.min).DefaultIfEmpty().Min();
            
            var localization = _labelText.localization;
            var matchString = subgroup.Type.GetMatchString(localization, minCategoryMatch, maxCategoryMatch);

            var maxAgreement = (float) GroupController.Group.MaxScore;

            var race = GroupController.RaceController.Race;

            var match = new LocalizationKey {fallback = matchString};
            var icon = race.IconWhite;
            var matchType = race.MatchType;
            var candidates = similarGroups.SelectMany(g => g.Candidates)
                .OrderByDescending(e => e.AgreementScore + e.match)
                .ToDictionary(
                    c => c,
                    c => c.CategoryMatch
                );

            var viewData = new SubgroupLeaderboardViewData(match, icon, matchType, candidates);
            GroupController.RaceController.Views.SubgroupLeaderboardView.Open(viewData);
        }
    }
}