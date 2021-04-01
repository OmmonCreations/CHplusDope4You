using DopeElections.ObstacleCourses;
using DopeElections.Races.Joker;
using Effects;
using Navigation;
using UnityEngine;

namespace DopeElections.Races
{
    [CreateAssetMenu(fileName = "Obstacle", menuName = "Dope Elections/Obstacles/Cannon Obstacle")]
    public class CannonObstacleType : JokerObstacleType
    {
        [Tooltip("Time for the load animation")] [SerializeField]
        private float _loadTime = 0.4f;

        [Tooltip("Randomization for the load animation")] [SerializeField]
        private float _loadTimeRandomization = 0.1f;

        [Tooltip("Time for the aim animation")] [SerializeField]
        private float _aimTime = 0.4f;

        [Tooltip("Randomization for the aim animation")] [SerializeField]
        private float _aimTimeRandomization = 0.1f;

        [Tooltip("Delay between the shoot animation starting and the candidate being ejected")] [SerializeField]
        private float _shootDelay = 0.4f;

        [Tooltip("Sets the camera shake strength")] [SerializeField]
        private float _impactStrength = 1;

        [Tooltip("Effect to play from the barrel when shooting")] [SerializeField]
        private EffectInstance _shootEffect = null;

        [Tooltip("Effect to play from the barrel when shooting")] [SerializeField]
        private EffectInstance _trailEffect = null;

        [SerializeField] private string _loadSound = "event:/sfx/obstacle/cannon/generic/load";
        [SerializeField] private string _shootSound = "event:/sfx/obstacle/cannon/generic/shoot";

        public float LoadTime => _loadTime;
        public float AimTime => _aimTime;
        public float LoadTimeRandomization => _loadTimeRandomization;
        public float AimTimeRandomization => _aimTimeRandomization;
        public float ShootDelay => _shootDelay;
        public float ImpactStrength => _impactStrength;
        public EffectInstance ShootEffect => _shootEffect;
        public EffectInstance TrailEffect => _trailEffect;
        public string LoadSound => _loadSound;
        public string ShootSound => _shootSound;

        public override RaceObstacle CreateObstacle(RaceObstacleCourse course, Vector2Int position,
            INavigationAgent user)
        {
            return new CannonObstacle(course, this, position, Size, user);
        }
    }
}