using System.Linq;
using Essentials;
using UnityEngine;

namespace DopeElections.Races.RaceTracks.Generators
{
    public class DynamicRaceTrackGenerator : IRaceTrackGenerator
    {
        private RaceTrackController RaceTrackController { get; }
        private RaceCameraController CameraController { get; }
        private RaceTrack RaceTrack { get; }

        private RaceController RaceController => CameraController.RaceController;

        public DynamicRaceTrackGenerator(RaceTrackController raceTrackController, RaceCameraController cameraController,
            RaceTrack raceTrack)
        {
            RaceTrackController = raceTrackController;
            CameraController = cameraController;
            RaceTrack = raceTrack;
        }

        public void UpdateTrackWidth()
        {
            var generator = RaceController.RaceTrack.MainGenerator;
            RaceTrack.ClearParts();
            generator.CreatePart();
        }

        /// <summary>
        /// Generates parts until track end is no longer within the camera view rect
        /// </summary>
        public void CreateParts()
        {
            if (IsTrackEndOutOfFrustum())
            {
                return;
            }
            const int limit = 10;
            var iteration = 0;
            var done = false;
            while (!done && iteration < limit)
            {
                iteration++;
                done = TryGeneratePart(out _);
            }
        }

        /// <summary>
        /// Generates parts until position is within track
        /// </summary>
        /// <param name="position"></param>
        public void CreateParts(float position)
        {
            const int limit = 10000;
            var iteration = 0;
            var done = false;
            while (!done && iteration < limit)
            {
                iteration++;
                var part = CreatePart();
                // if (part == null) Debug.LogError("There was an error when generating parts until " + position);
                // else Debug.Log("Generated part: " + part.StartPosition + " to " + part.EndPosition);
                done = part == null || part.EndPosition > position;
            }

            /*
            if (done)
            {
                Debug.Log("Generated parts until " + position+". Race track now reaches "+RaceTrack.Parts.Max(p=>p.EndPosition));
            }
            */
        }

        public RaceTrackPartInstance CreatePart()
        {
            TryGeneratePart(out var result);
            return result;
        }

        private bool TryGeneratePart(out RaceTrackPartInstance part)
        {
            var lastInstance = RaceTrack.Parts.LastOrDefault();
            var start = lastInstance != null ? lastInstance.EndPosition : 0;
            var instance = RaceTrackController.CreatePart(start);
            if (instance == null)
            {
                part = null;
                return true; // an error occurred if this happens
            }

            var transform = instance.Controller.transform;
            transform.localRotation = Quaternion.identity;
            transform.localPosition =
                RaceTrack.GetWorldPosition(new RaceTrackVector(0, start, RaceTrackVector.AxisType.Distance));
            // instance.Controller.GeneratePathfinderNodes(currentInstance != null ? currentInstance.Controller : null);
            // NavigationMesh.AddNodes(instance.Controller.PathfinderNodes);
            RaceTrack.AddPart(instance);

            part = instance;
            return true;
        }

        private bool IsTrackEndOutOfFrustum()
        {
            var part = RaceTrack.Parts.LastOrDefault();
            return part != null && part.Position > CameraController.CurrentPosition + CameraController.ViewLength;
        }

        private bool IsTrackPartInViewFrustum(RaceTrackPartInstance instance)
        {
            var cameraPosition = CameraController.CurrentPosition;
            var halfViewLength = CameraController.ViewLength / 2;
            var cameraViewStart = cameraPosition - halfViewLength;
            var cameraViewEnd = cameraPosition + halfViewLength;

            var start = instance.Position;
            var end = instance.EndPosition;
            return (start < cameraViewEnd && start > cameraViewStart) ||
                   (end < cameraViewEnd && end > cameraViewStart);
        }

        /// <summary>
        /// Disables parts that are far away
        /// </summary>
        public void UnloadParts()
        {
            var parts = RaceTrack.Parts.ToArray();
            for (var i = parts.Length - 1; i >= 0; i--)
            {
                var p = parts[i];
                if (p.Position > CameraController.CurrentPosition) continue;
                var visible = IsTrackPartInViewFrustum(p);
                // if (visible != p.Visible)
                // Debug.Log((visible ? "Show" : "Hide") + " part " + p.StartPosition + "|" + p.EndPosition + "!");
                if (visible) continue;
                p.Hide();
                // RaceTrack.RemovePartAt(i);
                /*
                var pathfinderNodes = p.Controller.PathfinderNodes;
                if (pathfinderNodes != null)
                {
                    NavigationMesh.RemoveNodes(pathfinderNodes, false);
                }*/
                // p.Controller.Remove();
            }
        }
    }
}