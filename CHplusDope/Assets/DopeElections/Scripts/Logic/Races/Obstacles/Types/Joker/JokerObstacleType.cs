using DopeElections.ObstacleCourses;
using Navigation;
using UnityEngine;

namespace DopeElections.Races.Joker
{
    public abstract class JokerObstacleType : RaceObstacleType
    {
        [SerializeField] private Vector2Int _size = Vector2Int.one;

        public Vector2Int Size => _size;

        public abstract RaceObstacle CreateObstacle(RaceObstacleCourse course, Vector2Int position,
            INavigationAgent user);
    }
}