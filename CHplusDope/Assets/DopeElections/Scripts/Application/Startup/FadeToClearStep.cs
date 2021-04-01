using BlackMasks;
using UnityEngine;

namespace DopeElections.Startup
{
    public class FadeToClearStep<T> : DopeElectionsStartupStep where T : MonoBehaviour, IBlackMaskProvider
    {
        public FadeToClearStep(DopeElectionsApp app) : base(app)
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
                Complete(true);
                return;
            }

            var blackMask = sceneController.BlackMask;
            blackMask.FadeToClear(() => Complete(true));
        }
    }
}