using System.Linq;
using DopeElections.Progression;
using DopeElections.Users;
using Essentials;
using Progression;
using Progression.UI;
using UnityEngine;

namespace DopeElections.Progress
{
    public class PlanetProgressController : ProgressionTreeController<PlanetProgressStepController>
    {
        [SerializeField] private Transform _meshAnchor = null;
        [SerializeField] private Transform _stepsAnchor = null;
        [SerializeField] private PlanetProgressStepController _stepTemplate = null;
        [SerializeField] private PlayerController _playerController = null;
        [SerializeField] private float _lastStepAngle = -34.5f;
        [SerializeField] private float _firstStepAngle = -214.469f;
        [SerializeField] private float _fromRotationAngle = 220;
        [SerializeField] private float _toRotationAngle = 37.9f;
        [SerializeField] private float _labelAngleDeltaMin = 0;
        [SerializeField] private float _labelAngleDeltaMax = 30;
        [SerializeField] private float _inputOffset = 0;
        [SerializeField] private PlanetRotationInputController _inputController = null;
        [SerializeField] private PlanetLoveMobileController _loveMobileController = null;

        [Header("Debug")] [SerializeField] private float _normalizedRotation;

        private float _rotation;

        public float Rotation => _rotation;
        public float NormalizedRotation => _normalizedRotation;

        public float FromRotationAngle => _fromRotationAngle;
        public float ToRotationAngle => _toRotationAngle;
        public PlanetLoveMobileController LoveMobileController => _loveMobileController;
        public Camera Camera => InputController.InteractionSystem.EventCamera;
        public PlayerController PlayerController => _playerController;

        private PlanetRotationInputController InputController => _inputController;
        private float FirstStepAngle => _firstStepAngle;
        private float LastStepAngle => _lastStepAngle;

        protected override void OnAwake()
        {
            base.OnAwake();
            _stepTemplate.gameObject.SetActive(false);
            PlayerController.ApplyUserConfiguration();
        }

        protected override void OnFocus(PlanetProgressStepController entry)
        {
            base.OnFocus(entry);
            var normalizedPosition = GetNormalizedPosition(entry);
            _inputController.GoTo(normalizedPosition);
            UpdateVisibleLabels();
        }

        protected override void OnFocusImmediate(PlanetProgressStepController entry)
        {
            base.OnFocus(entry);
            var normalizedPosition = GetNormalizedPosition(entry);
            _inputController.JumpTo(normalizedPosition);
            LoveMobileController.SetPositionImmediate(normalizedPosition);
            UpdateVisibleLabelsImmediate();
        }

        protected override void OnBeforeClearEntries()
        {
            PlacePlayer((PlanetProgressStepController) null);
            base.OnBeforeClearEntries();
        }

        protected override void OnInteractableChanged(bool interactable)
        {
            base.OnInteractableChanged(interactable);
            if(interactable) InputController.EnableMovement();
            else InputController.DisableMovement();
            LoveMobileController.Interactable = interactable;
        }

        public override void Select(PlanetProgressStepController entry)
        {
            if (entry.ExtraInfo != null)
            {
                TriggerEntrySelected(entry);
                return;
            }

            base.Select(entry);
        }

        public void SetRotation(float normalizedRotation)
        {
            ApplyRotation(normalizedRotation);
        }

        public void EnableMovement()
        {
            InputController.EnableMovement();
        }

        public void DisableMovement()
        {
            InputController.DisableMovement();
        }

        public void UpdateState()
        {
            ApplyRotation(NormalizedRotation);
            UpdateVisibleLabelsImmediate();
        }

        public void PlacePlayer(IVisibleProgressEntry entry)
        {
            var step = Controllers.FirstOrDefault(c => c.Entry == entry);
            PlacePlayer(step);
        }

        public void PlacePlayer(PlanetProgressStepController step)
        {
            var playerTransform = PlayerController.transform;
            if (step == null)
            {
                playerTransform.gameObject.SetActive(false);
                return;
            }

            if (!playerTransform.gameObject.activeSelf) playerTransform.gameObject.SetActive(true);
            playerTransform.SetParent(step.PlayerAnchor, false);
            playerTransform.localPosition = Vector3.zero;
            playerTransform.localRotation = Quaternion.identity;
            playerTransform.localScale = Vector3.one;
            PlayerController.PlayIdleLookingUpAnimation();
        }

        public float GetNormalizedPosition(IRaceProgressEntry entry)
        {
            return GetNormalizedPosition(Controllers.FirstOrDefault(c => c.Entry == entry));
        }

        public float GetNormalizedPosition(PlanetProgressStepController entry)
        {
            var index = Controllers.IndexOf(entry);
            return index / (float) (Controllers.Length - 1);
        }

        protected override bool IsLabelVisible(PlanetProgressStepController entry)
        {
            if (!ShowsLabels) return false;
            var normalizedPosition = GetNormalizedPosition(entry);
            var entryRotation = Mathf.Lerp(FirstStepAngle, LastStepAngle, normalizedPosition);
            var planetRotation = 360 - Rotation + _inputOffset;
            var minRotation = planetRotation - _labelAngleDeltaMin;
            var maxRotation = planetRotation + _labelAngleDeltaMax;

            return entryRotation >= minRotation && entryRotation <= maxRotation;
        }

        protected override ProgressEntryController[] CreateEntries(ProgressionTree tree)
        {
            var raceProgressionTree = tree as RaceProgressionTree;
            if (raceProgressionTree == null)
            {
                Debug.LogError("Cannot apply ProgressionTree of type " + tree.GetType().Name + " to " + GetType().Name +
                               "!");
                return null;
            }

            var raceEntries = tree.Entries.OfType<IVisibleProgressEntry>().ToList();
            var extraInfoPositions = raceProgressionTree.ExtraInfoPositions;
            var count = raceEntries.Count;
            var result = new ProgressEntryController[count];
            var fromAngle = _firstStepAngle;
            var toAngle = _lastStepAngle;

            var step = (toAngle - fromAngle) / Mathf.Max(1, (count - 1));

            for (var i = 0; i < count; i++)
            {
                var raceEntry = raceEntries[i];
                var extraInfoEntry = extraInfoPositions.TryGetValue(i, out var e) ? e : null;
                result[i] = CreateStep(raceEntry, extraInfoEntry, i, raceEntry.IsAvailable, fromAngle + i * step);
            }

            return result;
        }

        private PlanetProgressStepController CreateStep(IVisibleProgressEntry raceEntry, IExtraInfoEntry extraInfoEntry,
            int index, bool canShowLabel, float angle)
        {
            var instanceObject = Instantiate(_stepTemplate.gameObject, _stepsAnchor, false);
            var transform = instanceObject.transform;
            transform.localEulerAngles = new Vector3(angle, 0, 0);

            var instance = instanceObject.GetComponent<PlanetProgressStepController>();
            instance.Initialize(this, raceEntry, index, canShowLabel);
            instance.ExtraInfo = extraInfoEntry;
            instance.Interactable = Interactable;
            instance.HideLabelImmediate();

            instanceObject.SetActive(true);
            return instance;
        }

        private void ApplyRotation(float normalizedRotation)
        {
            var rotation = Mathf.Lerp(FromRotationAngle, ToRotationAngle, normalizedRotation);
            _rotation = rotation;
            _normalizedRotation = normalizedRotation;
            _meshAnchor.localEulerAngles = new Vector3(rotation, 0, 0);
            LoveMobileController.SetPosition(normalizedRotation);
            UpdateVisibleLabels();
        }
    }
}