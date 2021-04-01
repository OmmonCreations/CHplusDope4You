using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
    public class StudioEventEmitter : MonoBehaviour
    {
        public string Event
        {
            get
            {
                FMODPlaceholder.PrintWarning();
                return null;
            }
            set => FMODPlaceholder.PrintWarning();
        }
        
        public void Play()
        {
            FMODPlaceholder.PrintWarning();
        }
        
        public void Stop()
        {
            FMODPlaceholder.PrintWarning();
        }

        public void SetParameter(string volume, float p1)
        {
            FMODPlaceholder.PrintWarning();
        }
    }
}

