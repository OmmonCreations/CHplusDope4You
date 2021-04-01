using UnityEngine;

namespace DopeElections.Races
{
    public abstract class RaceObstacleType : ScriptableObject
    {
        [SerializeField] private RaceObstacleController _prefab = null;

        public RaceObstacleController Prefab => _prefab;
    }
}