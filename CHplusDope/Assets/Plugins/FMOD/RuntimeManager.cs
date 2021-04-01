using FMOD;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
    public class RuntimeManager
    {
        public static void PlayOneShot(string sound, Vector3 position)
        {
            FMODPlaceholder.PrintWarning();
        }
        
        public static void PlayOneShotAttached(string sound, GameObject gameObject)
        {
            FMODPlaceholder.PrintWarning();
        }
        
        public static FMOD.Studio.Bus GetBus(string path)
        {
            FMODPlaceholder.PrintWarning();
            return new Bus();
        }
    }
}