using System;
using System.Collections.Generic;
using System.Linq;
using BlackMasks;
using DopeElections.ObstacleCourses;
using DopeElections.PoliticalCharacters;
using DopeElections.Progression;
using DopeElections.Races.GroupLayout;
using DopeElections.Races.RaceTracks;
using Effects;
using Essentials;
using Localizator;
using MobileInputs;
using SpeechBubbles;
using StateMachines;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DopeElections.Races
{
    public abstract class RaceController : MonoBehaviour, IPoliticalCharacterEnvironment
    {
        public delegate void ResetEvent();

        public event ResetEvent Resetted = delegate { };

        #region Serialized Fields

        [FormerlySerializedAs("_menuController")] [Header("Prefab References")] [SerializeField]
        private RaceViewsContainer _views = null;

        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private BlackMask _whiteMask = null;
        [SerializeField] private RaceCandidateController _candidatePrefab = null;
        [SerializeField] private InteractionSystem _interactionSystem = null;
        [SerializeField] private EffectsController _effectsController = null;
        [SerializeField] private RaceOverlayController _overlayController = null;
        [SerializeField] private RaceObstacleCourseGeneratorAsset[] _obstacleCourseGenerators = null;

        [Header("Scene References")] [SerializeField]
        private RaceTrackController _raceTrackController = null;

        [SerializeField] private CandidateGroupController _groupController = null;
        [SerializeField] private RaceCameraController _cameraController = null;
        [SerializeField] private Transform _candidatesArea = null;
        [SerializeField] private SpeechBubbleLayer _speechBubbleLayer = null;
        [SerializeField] private LocalizationScope _localizationScope = null;

        #endregion

        #region Private Fields

        private RaceCandidateController[] _candidateControllers = null;

        #endregion

        #region Properties

        public RaceContext Context { get; private set; }
        public IRace Race { get; private set; }
        public RaceTrack RaceTrack { get; private set; }

        protected abstract int CurrentProgressIndex { get; }
        protected abstract IReadOnlyList<bool> ProgressSteps { get; }

        #endregion

        #region Auto Properties

        public RaceTrackController RaceTrackController => _raceTrackController;
        public RaceViewsContainer Views => _views;
        public CandidateGroupController GroupController => _groupController;
        public CandidateGroup CandidateGroup => GroupController.Group;
        public RaceCandidateController[] CandidateControllers => _candidateControllers;
        public Transform CandidatesArea => _candidatesArea;
        public InteractionSystem InteractionSystem => _interactionSystem;
        public EffectsController EffectsController => _effectsController;
        public RaceOverlayController OverlayController => _overlayController;
        public RaceCameraController CameraController => _cameraController;
        public ISpeechBubbleSource SpeechBubbleSource => _speechBubbleLayer;
        public ILocalization Localization => _localizationScope.Localization;
        public RaceObstacleCourseGeneratorAsset[] ObstacleCourseGenerators => _obstacleCourseGenerators;

        protected StateMachine StateMachine => _stateMachine;

        #endregion

        #region Initializers

        // executed on scene initialization
        public void Initialize()
        {
            CameraController.Initialize(this);
            CameraController.gameObject.SetActive(false);
            InteractionSystem.IgnoreUI = false;
        }

        public void PrepareRace(RaceContext context)
        {
            Context = context;
            Race = context.Race;

            RaceTrack = new RaceTrack(GetTrackPartSet(context));

            OnBeforeRacePrepare();

            _whiteMask.BlockInteractions(false);
            RaceTrackController.Initialize(RaceTrack);
            PrepareOverlay();
            PrepareProgressDisplay();

            StartTrackGenerator(70);
            SpawnCandidates();
            GroupController.Stroll();
            CameraController.gameObject.SetActive(true);
            CameraController.FollowGroup();

            _whiteMask.Alpha = 0;

            OnRacePrepared();
        }

        protected virtual void OnBeforeRacePrepare()
        {
        }

        protected virtual void OnRacePrepared()
        {
        }

        private void PrepareOverlay()
        {
            OverlayController.Title = Race.Label;
            OverlayController.BackAction = Cancel;
            OverlayController.OpenHelpAction = OpenHelp;
            OverlayController.HideImmediate();
        }

        // executed every time a marathon starts
        private void PrepareProgressDisplay()
        {
            OverlayController.ProgressDisplay.Initialize(ProgressSteps, OnProgressEntrySelected);
        }

        public abstract void StartRace();

        #endregion

        #region Unity Control

        protected void Update()
        {
            StateMachine.Run();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Resets all candidates to their correct position according to the current marathon, resets the track to
        /// 0 clearing all generated parts, updates the track width, resets the camera and puts the candidate group
        /// into a strolling state
        /// </summary>
        public void SoftReset()
        {
            CandidateGroup.RecalculateGroups();
            CameraController.CurrentPosition = 0;
            RaceTrack.MainGenerator.UpdateTrackWidth();
            OverlayController.ProgressDisplay.JumpTo(CurrentProgressIndex);

            CandidateGroup.ResetState(200);
            GroupController.Stroll();

            PrepareProgressDisplay();

            OnSoftReset();
            Resetted();
            
            RaceTrack.MainGenerator.CreateParts();
        }

        protected virtual void OnSoftReset()
        {
        }

        public void OpenHelp()
        {
            DopeElectionsApp.Instance.Views.RaceInfoView.Open();
        }

        public void Cancel()
        {
            Finish(false);
        }

        public void Complete()
        {
            Finish(true);
        }

        private void Finish(bool completed)
        {
            OverlayController.HideImmediate();
            if (completed)
            {
                _whiteMask.BlockInteractions(true);
                _whiteMask.FadeToBlack(2, () => FinishRace(true));
            }
            else
            {
                FinishRace(false);
            }
        }

        protected abstract void FinishRace(bool completed);

        public void Unload()
        {

            if (_candidateControllers != null)
            {
                foreach (var c in _candidateControllers) c.Remove();
            }
            
            CameraController.gameObject.SetActive(false);

            _raceTrackController.Unload();
            _groupController.Unload();
            
            _candidateControllers = null;

            Context = null;
            Race = null;
            RaceTrack = null;
        }

        public void StartTrackGenerator(float startLength)
        {
            var generator = RaceTrackController.InitializeGenerator(CameraController);
            generator.CreateParts(startLength);
        }

        public void SpawnCandidates()
        {
            var context = Context;
            var race = Race;
            var raceTrack = RaceTrack;
            var width = raceTrack.Width;
            var composition = GroupController.Composition;
            var candidateConfiguration = RaceCandidateConfiguration.Default;
            var candidates = race.Candidates;
            var controllers = SpawnCandidates(candidates);
            var group = new CandidateGroup(context, raceTrack, composition, candidateConfiguration);
            var layout = GroupLayoutSolver.CalculateLayoutFixedWidth(group, width, group.Groups);
            _candidateControllers = controllers;
            GroupController.Initialize(this, group, layout.Length);
            SoftReset();
        }

        #endregion

        #region Event Listeners

        protected abstract void OnProgressEntrySelected(int index);

        #endregion

        #region Helper Methods

        private RaceTrackPartSet GetTrackPartSet(RaceContext context)
        {
            var candidatesCount = context.Race.Candidates.Length;
            var composition = GroupController.Composition;
            var maxCandidates = composition.CalculateMaxCandidates(candidatesCount, context.RelativeRaceIndex);
            var layoutConfiguration = GroupController.Composition.LayoutConfiguration;
            var preferredColumnCount =
                Mathf.Sqrt(maxCandidates * (1 + layoutConfiguration.EmptySlotPercentage) * 1.3f);
            var preferredWidth = layoutConfiguration.SlotSize * preferredColumnCount;
            return RaceTrackController.Sets.OrderBy(s => Mathf.Abs(s.Width - preferredWidth)).FirstOrDefault();
        }

        private RaceCandidateController[] SpawnCandidates(RaceCandidate[] candidates)
        {
            var width = RaceTrack.Width;

            var amount = candidates.Length;
            var startSpread = amount / width;
            var result = new RaceCandidateController[amount];
            var i = 0;
            foreach (var candidate in candidates)
            {
                var startXPosition = Random.Range(0, width);
                var startZPosition = Random.Range(0, startSpread);
                var startPosition = new Vector3(-width / 2 + startXPosition, 0, startZPosition);
                result[i] = SpawnCandidate(startPosition, candidate);
                i++;
            }

            return result;
        }

        private RaceCandidateController SpawnCandidate(Vector3 startPosition, RaceCandidate candidate)
        {
            var instanceObject = Instantiate(_candidatePrefab.gameObject, _candidatesArea, false);
            instanceObject.transform.localPosition = startPosition;

            var instance = instanceObject.GetComponent<RaceCandidateController>();
            instance.Initialize(this, candidate);
            instance.LOD = RaceTrack.PartsSet.CandidateLod;

            return instance;
        }

        #endregion
    }
}