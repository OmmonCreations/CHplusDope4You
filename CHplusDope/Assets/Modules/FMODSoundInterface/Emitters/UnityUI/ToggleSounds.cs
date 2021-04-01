using UnityEngine;
using UnityEngine.UI;

namespace FMODSoundInterface.UnityUI
{
    public class ToggleSounds : AbstractSoundEmitter<Toggle>
    {
        public string changeEvent = UISound.Toggle.Change;
        public string enabledEvent = UISound.Toggle.Enabled;
        public string disabledEvent = UISound.Toggle.Disabled;

        [SerializeField] private bool _playChange = true;
        [SerializeField] private bool _playEnabled = false;
        [SerializeField] private bool _playDisabled = false;

        [Header("Auto-Link (optional)")] [SerializeField]
        private Toggle _source = null;

        protected override Toggle Source => _source;

        public override void HookEvents(Toggle source)
        {
            source.onValueChanged.AddListener(Change);
        }

        public override void ReleaseHooks(Toggle source)
        {
            source.onValueChanged.RemoveListener(Change);
        }

        private void Change(bool value)
        {
            if (_playChange) Change();
            if (_playEnabled && value) Enabled();
            else if (_playDisabled && !value) Disabled();
        }

        public void Change()
        {
            SoundController.Play(changeEvent);
        }

        public void Enabled()
        {
            SoundController.Play(enabledEvent);
        }

        public void Disabled()
        {
            SoundController.Play(disabledEvent);
        }
    }
}