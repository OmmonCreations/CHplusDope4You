using BlackMasks;
using UnityEngine;

namespace DopeElections.Startup
{
    public class FadeToBlackStep<T> : DopeElectionsStartupStep where T : MonoBehaviour, IBlackMaskProvider
    {
        public FadeToBlackStep(DopeElectionsApp app) : base(app)
        {
        }

        public override void Run(bool data)
        {
            if (!data)
            {
                Complete(false);
                return;
            }
            var sceneController = Object.FindObjectOfType<T>() as IBlackMaskProvider;
            if (sceneController == null)
            {
                Debug.LogWarning("No BlackMask provider found.");
                Complete(true);
                return;
            }

            var blackMask = sceneController.BlackMask;
            blackMask.FadeToBlack(() => Complete(true));
        }
    }
}