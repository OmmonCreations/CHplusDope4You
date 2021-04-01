using UnityEngine;

namespace DopeElections.Races.Hurdle
{
    [CreateAssetMenu(fileName = "Obstacle", menuName = "Dope Elections/Obstacles/Hurdle Obstacle")]
    public class HurdleObstacleType : RaceObstacleType
    {
        [Tooltip("Tile footprint of one hurdle")] [SerializeField]
        private Vector2Int _size = Vector2Int.one;

        [Tooltip("Where to put the mesh transform relative to the footprint")] [SerializeField]
        private Vector2 _pivot = Vector2.zero;

        [Tooltip("Jump height for candidates to cross the hurdle")] [SerializeField]
        private float _jumpHeight = 3;

        [Tooltip("Jump height for candidates to crash into the hurdle")] [SerializeField]
        private float _crashHeight = 3;

        [Tooltip("Sound to play when a candidate successfully clears the hurdle")] [SerializeField]
        private string _clearedSound = "event:/sfx/obstacle/hurdle/generic/cleared";

        [SerializeField] private string _droppedSound = "event:/sfx/obstacle/hurdle/generic/dropped";

        public Vector2Int Size => _size;
        public Vector2 Pivot => _pivot;
        public float JumpHeight => _jumpHeight;
        public float CrashHeight => _crashHeight;
        public string ClearedSound => _clearedSound;
        public string DroppedSound => _droppedSound;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_size.x <= 0 || _size.y <= 0) _size = new Vector2Int(Mathf.Max(1, _size.x), Mathf.Max(1, _size.y));
        }
#endif
    }
}