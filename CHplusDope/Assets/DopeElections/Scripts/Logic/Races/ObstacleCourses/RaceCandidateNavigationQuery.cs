using DopeElections.Races;
using Navigation;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    public class RaceCandidateNavigationQuery : NavigationQuery
    {
        public ReactionData ReactionData { get; }

        public RaceCandidateNavigationQuery(Vector2Int @from, Vector2Int to, float timestamp, bool precise,
            ReactionData reactionData) : base(@from, to, timestamp, precise)
        {
            ReactionData = reactionData;
        }
    }
}