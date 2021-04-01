using UnityEngine;

namespace DopeElections.Races.RaceTracks
{
    public class DynamicRaceTrackState : RaceTrackState
    {
        private const float UnloadInterval = 2;
        
        private IRaceTrackGenerator Generator { get; }
        private RaceCameraController CameraController { get; }

        private Transform PartsAnchor { get; }

        // public NavigationMesh NavigationMesh { get; }

        private float _unloadTimeout = UnloadInterval;
        
        public DynamicRaceTrackState(RaceTrackController raceTrack, RaceCameraController cameraController)
            : base(raceTrack)
        {
            Generator = raceTrack.RaceTrack.MainGenerator;
            CameraController = cameraController;
            PartsAnchor = raceTrack.PartsAnchor;
            // NavigationMesh = new NavigationMesh();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            CameraController.PositionChanged += OnCameraPositionChanged;
        }

        public override void Update()
        {
            if (_unloadTimeout > 0) _unloadTimeout -= Time.deltaTime;
            if (_unloadTimeout <= 0)
            {
                Generator.UnloadParts();
                _unloadTimeout = UnloadInterval;
            }
        }

        private void OnCameraPositionChanged(float position)
        {
            Generator.CreateParts();
        }

        /*
        public void GenerateStart()
        {
            GenerateStart(_currentKey);
        }

        private void GenerateStart(NamespacedKey key)
        {
            const int limit = 10;
            var iterations = 0;
            while (iterations < limit)
            {
                iterations++;
                var part = RaceTrackController.CreatePart(key, key);
                if (part == null) return;
                var waypointOut = part.WaypointOut;
                var next = RaceTrack.Parts.FirstOrDefault();
                var nextWaypoint = next != null ? next.Controller.WaypointIn : RaceTrackController.StartWaypoint;
                var nextPosition = nextWaypoint.Position;
                var nextRotation = nextWaypoint.Rotation;

                var transform = part.transform;
                var rotation = nextRotation * (Quaternion.Inverse(waypointOut.Rotation) * transform.rotation);
                transform.rotation = rotation;
                transform.position = nextPosition + (transform.position - waypointOut.Position);
                part.RecalculateBounds();
                var instance = new RaceTrackPartInstance(part, 0);
                if (!IsTrackPartInViewFrustum(instance) && false)
                {
                    part.Remove();
                    break;
                }

                RaceTrack.InsertPart(0, instance);
            }
        }
        */

        protected override void OnFinish()
        {
            base.OnFinish();
            RaceTrack.ClearParts();
            RaceTrackController.ClearParts();
            if (CameraController) CameraController.PositionChanged -= OnCameraPositionChanged;
        }
    }
}