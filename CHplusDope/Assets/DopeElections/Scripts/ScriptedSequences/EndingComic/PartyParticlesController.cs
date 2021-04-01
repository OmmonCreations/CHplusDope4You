using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DopeElections.ScriptedSequences.EndingComic
{
    public class PartyParticlesController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _particleSystems = null;

        private float _timeout = 0;
        private const float MaxTimout = 3f;

        private void Update()
        {
            _timeout -= Time.deltaTime;
            if (_timeout > 0) return;
            _timeout = Mathf.Pow(Random.Range(0, 1), 2) * MaxTimout;
            var availableParticleSystems = _particleSystems.Where(p => !p.isPlaying).ToList();
            if (availableParticleSystems.Count == 0) return;
            availableParticleSystems[Random.Range(0, availableParticleSystems.Count)].Play();
        }
    }
}