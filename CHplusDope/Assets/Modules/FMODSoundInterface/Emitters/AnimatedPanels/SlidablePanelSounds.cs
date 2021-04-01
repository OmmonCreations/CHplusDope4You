using AnimatedObjects;
using UnityEngine;

namespace FMODSoundInterface.AnimatedPanels
{
    public class SlidablePanelSounds : AbstractSoundEmitter<SlidablePanelController>
    {
        public string slideInEvent = PanelSound.Panel.SlideIn;
        public string slideOutEvent = PanelSound.Panel.SlideOut;

        [Header("Auto-Link (optional)")] [SerializeField]
        private SlidablePanelController _source = null;

        protected override SlidablePanelController Source => _source;

        public override void HookEvents(SlidablePanelController source)
        {
            _source.Appears += SlideIn;
            _source.Disappears += SlideOut;
        }

        public override void ReleaseHooks(SlidablePanelController source)
        {
            _source.Appeared -= SlideIn;
            _source.Disappeared -= SlideOut;
        }

        private void SlideIn(float time) => SlideIn();
        private void SlideOut(float time) => SlideOut();

        public void SlideIn()
        {
            SoundController.Play(slideInEvent);
        }

        public void SlideOut()
        {
            SoundController.Play(slideOutEvent);
        }
    }
}