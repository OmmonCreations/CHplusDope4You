using System.Collections.Generic;
using UnityEngine;

namespace Localizator
{
    public sealed class DefaultLocalization : ILocalization
    {
        public string GetString(LocalizationKey key)
        {
            return key.fallback != null ? key.fallback : key.path;
        }

        public bool TryGetString(LocalizationKey key, out string value)
        {
            value = key.fallback != null ? key.fallback : key.path;
            return false;
        }

        public void SetString(LocalizationKey key, string value)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Cannot edit default localization.");
#endif
        }
    }
}