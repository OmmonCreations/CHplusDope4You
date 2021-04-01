using System.Linq;
using Essentials;
using UnityEngine;

namespace Effects
{
    public class EffectLibrary : MonoBehaviour
    {
        [SerializeField] private EffectInstance[] _effects = null;

        public EffectInstance GetEffect(NamespacedKey key)
        {
            return _effects.FirstOrDefault(e => e.TypeId == key);
        }
    }
}