using Effects;
using UnityEngine;

namespace DopeElections.Races
{
    [CreateAssetMenu(fileName = "Obstacle", menuName = "Dope Elections/Obstacles/Impact Obstacle")]
    public class ImpactObstacleType : RaceObstacleType
    {
        [Tooltip("Interval of impacts in seconds")] [SerializeField]
        private float _frequency = 5;

        [Tooltip("Delay of the first impact in seconds")] [SerializeField]
        private float _delay = 0.5f;

        [Tooltip("Randomization of the first impact delay in seconds")] [SerializeField]
        private float _randomizeDelay = 0.5f;

        [Tooltip(
            "Duration of the impact in seconds (prevents candidates from passing and keeps squished candidates stunned)")]
        [SerializeField]
        private float _duration = 0.5f;

        [Tooltip("Duration of the stun when hit in seconds (candidates remain stunned if duration is larger)")]
        [SerializeField]
        private float _stunDuration = 1;

        [Tooltip("Sets the camera shake strength")] [SerializeField]
        private float _impactStrength = 1;

        [Tooltip("Size of the impact area in tiles")] [SerializeField]
        private Vector2Int _size = Vector2Int.one;

        [Tooltip("Where to put the mesh relative to the impact area when placed on the left side of the track")]
        [SerializeField]
        private Vector2 _leftPivot = new Vector2(0, 0.5f);

        [Tooltip("Where to put the mesh relative to the impact area when placed on the inside of the track")]
        [SerializeField]
        private Vector2 _centerPivot = new Vector2(0.5f, 0.5f);

        [Tooltip("Where to put the mesh relative to the impact area when placed on the right side of the track")]
        [SerializeField]
        private Vector2 _rightPivot = new Vector2(1, 0.5f);

        [Tooltip("The SFX to play when an impact occurs")] [SerializeField]
        private EffectInstance _impactEffect = null;

        [SerializeField] private string _impactSound = "event:/sfx/obstacle/impact/generic/impact";

        /// <summary>
        /// Interval of impacts in seconds
        /// </summary>
        public float Frequency => _frequency;

        /// <summary>
        /// Delay of the first impact in seconds
        /// </summary>
        public float Delay => _delay;

        /// <summary>
        /// Randomization of the first impact delay in seconds
        /// </summary>
        public float RandomizeDelay => _randomizeDelay;

        /// <summary>
        /// Duration of the impact in seconds (prevents candidates from passing and keeps squished candidates stunned)
        /// </summary>
        public float Duration => _duration;

        /// <summary>
        /// Duration of the stun when hit in seconds (candidates remain stunned if duration is larger)
        /// </summary>
        public float StunDuration => _stunDuration;

        /// <summary>
        /// Sets the camera shake strength
        /// </summary>
        public float ImpactStrength => _impactStrength;

        /// <summary>
        /// Size of the impact area in tiles
        /// </summary>
        public Vector2Int Size => _size;

        /// <summary>
        /// Where to put the mesh relative to the impact area when placed on the left side of the track
        /// </summary>
        public Vector2 LeftPivot => _leftPivot;

        /// <summary>
        /// Where to put the mesh relative to the impact area when placed on the inside of the track
        /// </summary>
        public Vector2 CenterPivot => _centerPivot;

        /// <summary>
        /// Where to put the mesh relative to the impact area when placed on the right side of the track
        /// </summary>
        public Vector2 RightPivot => _rightPivot;

        /// <summary>
        /// The SFX to play when an impact occurs
        /// </summary>
        public EffectInstance ImpactEffect => _impactEffect;

        public string ImpactSound => _impactSound;

        public Vector2 GetPivot(ImpactObstacle.TrackAnchor anchor)
        {
            switch (anchor)
            {
                case ImpactObstacle.TrackAnchor.Left: return LeftPivot;
                case ImpactObstacle.TrackAnchor.Center: return CenterPivot;
                case ImpactObstacle.TrackAnchor.Right: return RightPivot;
                default: return new Vector2(0.5f, 0.5f);
            }
        }
    }
}