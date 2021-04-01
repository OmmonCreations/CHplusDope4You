using UnityEngine;

namespace DopeElections.Progress
{
    public class LeaderboardControls : MonoBehaviour
    {
        [SerializeField] private LeaderboardControl[] _controls = null;

        public void Initialize(LeaderboardController leaderboard)
        {
            foreach(var c in _controls) c.Initialize(leaderboard);
        }

        public void UpdateControls()
        {
            foreach (var c in _controls) c.UpdateControl();
        }
    }
}