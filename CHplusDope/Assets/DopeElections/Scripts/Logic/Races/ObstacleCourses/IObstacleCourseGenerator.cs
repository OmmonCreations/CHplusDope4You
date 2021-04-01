using System.Collections.Generic;
using Navigation;
using UnityEngine;

namespace DopeElections.ObstacleCourses
{
    public interface IObstacleCourseGenerator
    {
        float TileSize { get; }
        void Generate(RaceObstacleCourse course, IEnumerable<KeyValuePair<INavigationAgent,Vector2Int>> jokerUsers);
        int GetPreferredObstacleSpaceLength(int width);
    }
}