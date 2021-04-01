using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DopeElections.Races
{
    public class CameraFollowGroupState : RaceCameraState
    {
        private CandidateGroupController GroupController { get; }

        public RaceCandidateController[] Candidates { get; }

        public CameraFollowGroupState(RaceCameraController controller,
            IEnumerable<RaceCandidateController> candidates) :
            base(controller)
        {
            GroupController = controller.RaceController.GroupController;
            Candidates = candidates.ToArray();
        }

        public override void Update()
        {
            var candidates = Candidates;
            var frontrunnerPosition = candidates.Where(c => c.IsVisible)
                .Select(c => c.Position.y)
                .DefaultIfEmpty(0)
                .Max();
            var backlinePosition = candidates.Where(c => c.IsVisible)
                .Select(c => c.Position.y)
                .DefaultIfEmpty(0)
                .Min();
            var viewLength = CameraController.ViewLength;
            var groupLength = frontrunnerPosition - backlinePosition;

            var offset = CameraController.CalculateFrontOffset(groupLength, viewLength);
            var targetPosition = frontrunnerPosition - offset;

            /*
            Debug.Log("Frontrunner: " + frontrunnerPosition + "\n" +
                      "Offset from View length: " + frontOffsetFromViewLength + "\n" +
                      "Offset from group length: " + frontOffsetFromGroupLength + "\n" +
                      "Position: " + targetPosition);
                      */

            CameraController.TargetPosition = Mathf.Max(CameraController.TargetPosition, targetPosition);
        }
    }
}