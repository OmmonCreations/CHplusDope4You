using System.Collections.Generic;
using DopeElections.Races.Joker;
using Navigation;
using UnityEngine;
using Random = RandomUtils.Random;

namespace DopeElections.ObstacleCourses
{
    public abstract class RaceObstacleCourseGeneratorAsset : ScriptableObject, IObstacleCourseGenerator
    {
        [SerializeField] private KeyCode _hotkey = KeyCode.None;
        [SerializeField] private float _tileSize = 1.5f;
        [SerializeField] private SmartSpiderAxisAssociation _axisAssociation = null;
        [SerializeField] private JokerObstacleType[] _jokerObstacleTypes = null;

        public KeyCode Hotkey => _hotkey;
        public float TileSize => _tileSize;
        public SmartSpiderAxisAssociation AxisAssociation => _axisAssociation;

        public void Generate(RaceObstacleCourse course,
            IEnumerable<KeyValuePair<INavigationAgent, Vector2Int>> jokerUsers)
        {
            Generate(course);
            if (jokerUsers != null) GenerateJokerObstacles(course, jokerUsers);
        }

        protected abstract void Generate(RaceObstacleCourse course);

        public abstract int GetPreferredObstacleSpaceLength(int width);

        /// <summary>
        /// Places a joker obstacle next to each candidate supplied
        /// </summary>
        private void GenerateJokerObstacles(RaceObstacleCourse course,
            IEnumerable<KeyValuePair<INavigationAgent, Vector2Int>> entries)
        {
            var jokerObstacleTypes = _jokerObstacleTypes;
            if (jokerObstacleTypes == null || jokerObstacleTypes.Length == 0) return;

            foreach (var entry in entries)
            {
                var user = entry.Key;
                var position = entry.Value;
                var type = jokerObstacleTypes[Random.Range(0, jokerObstacleTypes.Length)];
                var obstacle = type.CreateObstacle(course, position, user);
                course.AddObstacle(obstacle);
            }
        }
    }
}