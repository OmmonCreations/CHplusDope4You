using UnityEngine;

namespace DopeElections.Progress
{
    public abstract class LeaderboardControl : MonoBehaviour
    {
        public LeaderboardController Leaderboard { get; private set; }
        
        public void Initialize(LeaderboardController controller)
        {
            Leaderboard = controller;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }

        public abstract void UpdateControl();
    }
}