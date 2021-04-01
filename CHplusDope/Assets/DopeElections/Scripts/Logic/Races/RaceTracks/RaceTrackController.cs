using System.Collections.Generic;
using System.Linq;
using DopeElections.Races.RaceTracks.Generators;
using Essentials;
using StateMachines;
using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    public class RaceTrackController : MonoBehaviour
    {
        [SerializeField] private Transform _root = null;
        [SerializeField] private StateMachine _stateMachine = null;
        [SerializeField] private Transform _partsAnchor = null;
        [SerializeField] private Transform _candidatesAnchor = null;
        [SerializeField] private RaceTrackPartSet[] _sets = null;

        // private NavigationMesh _navigationMesh;

        public Transform PartsAnchor => _partsAnchor;

        public Transform CandidatesAnchor => _candidatesAnchor;
        // public NavigationMesh NavigationMesh => _navigationMesh;

        public RaceTrackPartSet[] Sets => _sets;

        private List<RaceTrackPartController> _controllers = new List<RaceTrackPartController>();

        private DynamicRaceTrackState _generatorState;
        private Vector3 _center;

        public Vector3 Center
        {
            get => _center;
            set => ApplyCenter(value);
        }

        public RaceTrack RaceTrack { get; private set; }
        public StateMachine StateMachine => _stateMachine;
        public Transform Root => _root;

        public void Initialize(RaceTrack raceTrack)
        {
            RaceTrack = raceTrack;
            HookEvents();
        }

        private void Update()
        {
            StateMachine.Run();
        }

        private void OnDestroy()
        {
            ReleaseHooks();
        }

        private void HookEvents()
        {
            if (RaceTrack != null)
            {
                RaceTrack.PartAdded += OnPartAdded;
                RaceTrack.PartRemoved += OnPartRemoved;
            }
        }

        private void ReleaseHooks()
        {
            if (RaceTrack != null)
            {
                RaceTrack.PartAdded -= OnPartAdded;
                RaceTrack.PartRemoved -= OnPartRemoved;
            }
        }

        public IRaceTrackGenerator InitializeGenerator(RaceCameraController cameraController)
        {
            var raceTrack = RaceTrack;
            
            var result = new DynamicRaceTrackGenerator(this, cameraController, raceTrack);
            raceTrack.MainGenerator = result;
            
            var state = new DynamicRaceTrackState(this, cameraController);
            _generatorState = state;
            StateMachine.State = state;
            
            return result;
        }

        public void ClearGenerator()
        {
            if (_generatorState == null) return;
            ClearParts();
        }

        public void StopGenerator()
        {
            StateMachine.State = null;
        }

        public void Unload()
        {
            StopGenerator();
            ClearGenerator();
        }

        public RaceTrackPartInstance CreatePart(float position)
        {
            var part = CreatePart();
            if (!part) return null;
            part.Position = position;
            var result = new RaceTrackPartInstance(part, position);
            return result;
        }

        private RaceTrackPartController CreatePart()
        {
            var set = RaceTrack.PartsSet;

            if (set.Parts.Length == 0)
            {
                Debug.LogError("RaceTrackPartSet " + set.name + " contains 0 track templates!");
                return null;
            }

            var part = set.Parts[UnityEngine.Random.Range(0, set.Parts.Length)];
            var resultObject = Instantiate(part.gameObject, PartsAnchor, false);
            resultObject.SetActive(true);
            return resultObject.GetComponent<RaceTrackPartController>();
        }

        public RaceTrackPartInstance GetPart(float position, bool generate = true)
        {
            if (position >= RaceTrack.MaxPosition && generate && _generatorState != null)
            {
                RaceTrack.MainGenerator.CreateParts(position + 1);
            }

            return RaceTrack.GetPart(position);
        }

        public void ClearParts()
        {
            foreach (var c in _controllers) c.Remove();
            _controllers.Clear();
        }

        private void ApplyCenter(Vector3 center)
        {
            _center = center;
            _root.position = -center;
        }

        private void OnPartAdded(RaceTrackPartInstance instance)
        {
            _controllers.Add(instance.Controller);
        }

        private void OnPartRemoved(RaceTrackPartInstance instance)
        {
            _controllers.Remove(instance.Controller);
            instance.Controller.Remove();
        }
    }
}