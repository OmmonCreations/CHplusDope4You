using UnityEngine;

namespace FMOD.Studio
{
    public static class FMODPlaceholder
    {
        private static bool _warningPrinted = false;
        
        public static void PrintWarning()
        {
            if (_warningPrinted) return;
            _warningPrinted = true;
            
            Debug.LogWarning("Download and install the FMOD Unity Plugin from https://fmod.com");
        }
    }
}