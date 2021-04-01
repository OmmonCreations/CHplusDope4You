using AnimatedObjects;
using UnityEngine;

namespace FMODSoundInterface.AnimatedPanels
{
    public class PoppablePanelSounds : AbstractSoundEmitter<PoppablePanelController>
    {
        public string popInEvent = PanelSound.Panel.PopIn;
        public string popOutEvent = PanelSound.Panel.PopOut;

        [Header("Auto-Link (optional)")] [SerializeField]
        private PoppablePanelController _source = null;

        protected override PoppablePanelController Source => _source;

        public override void HookEvents(PoppablePanelController source)
        {
            _source.Appears += PopIn;
            _source.Disappears += PopOut;
        }

        public override void ReleaseHooks(PoppablePanelController source)
        {
            _source.Appeared -= PopIn;
            _source.Disappeared -= PopOut;
        }

        private void PopIn(float time) => PopIn();
        private void PopOut(float time) => PopOut();

        public void PopIn()
        {
            SoundController.Play(popInEvent);
        }

        public void PopOut()
        {
            SoundController.Play(popOutEvent);
        }
    }
}