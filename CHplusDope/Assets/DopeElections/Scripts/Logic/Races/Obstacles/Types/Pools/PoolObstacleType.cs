using Effects;
using UnityEngine;

namespace DopeElections.Races
{
    [CreateAssetMenu(fileName = "Obstacle", menuName = "Dope Elections/Obstacles/Pool Obstacle")]
    public class PoolObstacleType : RaceObstacleType
    {
        [SerializeField] private float _speedModifier = 0.2f;
        [SerializeField] private MovementParticleEffect _movementParticles = null;
        [SerializeField] private ParticleEffectInstance _toppleParticles = null;
        [SerializeField] private string _splashSound = "event:/sfx/obstacle/pool/generic/splash";

        public float SpeedModifier => _speedModifier;
        public MovementParticleEffect MovementParticles => _movementParticles;
        public ParticleEffectInstance ToppleParticles => _toppleParticles;
        public string SplashSound => _splashSound;
    }
}