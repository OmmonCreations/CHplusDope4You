using BlackMasks;
using UnityEngine;

namespace FMODSoundInterface.BlackMasks
{
    public class BlackMaskSounds : AbstractSoundEmitter<BlackMask>
    {
        public string fadeToBlackEvent = TransitionSound.FadeToBlack;
        public string fadeToClearEvent = TransitionSound.FadeToBlack;
        
        [Header("Auto-Link (optional)")]
        [SerializeField] private BlackMask _source = null;

        protected override BlackMask Source => _source;

        public override void HookEvents(BlackMask source)
        {
            _source.FadesToBlack += FadeToBlack;
            _source.FadesToClear += FadeToClear;
        }

        public override void ReleaseHooks(BlackMask source)
        {
            _source.FadesToBlack -= FadeToBlack;
            _source.FadesToClear -= FadeToClear;
        }

        private void FadeToBlack(float time) => FadeToBlack();
        private void FadeToClear(float time) => FadeToClear();

        public void FadeToBlack()
        {
            SoundController.Play(fadeToBlackEvent);
        }

        public void FadeToClear()
        {
            SoundController.Play(fadeToClearEvent);
        }
    }
}