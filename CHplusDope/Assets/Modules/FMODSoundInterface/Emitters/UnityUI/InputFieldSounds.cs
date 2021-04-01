using UnityEngine;
using UnityEngine.UI;

namespace FMODSoundInterface.UnityUI
{
    public class InputFieldSounds : AbstractSoundEmitter<InputField>
    {
        public string blurEvent = UISound.InputField.Blur;
        public string changeEvent = UISound.InputField.Change;
        
        [Header("Auto-Link (optional)")]
        [SerializeField] private InputField _source = null;

        protected override InputField Source => _source;

        public override void HookEvents(InputField source)
        {
            source.onValueChanged.AddListener(Change);
            source.onEndEdit.AddListener(Blur);
        }

        public override void ReleaseHooks(InputField source)
        {
            source.onValueChanged.RemoveListener(Change);
            source.onEndEdit.RemoveListener(Blur);
        }

        private void Blur(string value) => Blur();
        private void Change(string value) => Change();

        public void Blur()
        {
            SoundController.Play(blurEvent);
        }

        public void Change()
        {
            SoundController.Play(changeEvent);
        }
    }
}