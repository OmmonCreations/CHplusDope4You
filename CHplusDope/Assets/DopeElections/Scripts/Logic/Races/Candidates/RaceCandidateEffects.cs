using UnityEngine;

namespace DopeElections.Races
{
    public class RaceCandidateEffects : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _spawnParticles = null;
        [SerializeField] private ParticleSystem _despawnParticles = null;

        public void PlaySpawnParticles()
        {
            _spawnParticles.Play();
        }

        public void PlayDespawnParticles()
        {
            _despawnParticles.Play();
        }
    }
}