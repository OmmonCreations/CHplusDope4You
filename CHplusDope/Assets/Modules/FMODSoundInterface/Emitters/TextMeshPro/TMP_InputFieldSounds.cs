using TMPro;
using UnityEngine;

namespace FMODSoundInterface.TextMeshPro
{
    public class TMP_InputFieldSounds : AbstractSoundEmitter<TMP_InputField>
    {
        public string blurEvent = TextMeshProSound.InputField.Blur;
        public string changeEvent = TextMeshProSound.InputField.Change;
        public string focusEvent = TextMeshProSound.InputField.Focus;
        public string submitEvent = TextMeshProSound.InputField.Submit;
        
        [Header("Auto-Link (optional)")]
        [SerializeField] private TMP_InputField _source = null;

        protected override TMP_InputField Source => _source;

        public override void HookEvents(TMP_InputField source)
        {
            source.onDeselect.AddListener(Blur);
            source.onValueChanged.AddListener(Change);
            source.onSelect.AddListener(Focus);
            source.onSubmit.AddListener(Submit);
        }

        public override void ReleaseHooks(TMP_InputField source)
        {
            source.onDeselect.RemoveListener(Blur);
            source.onValueChanged.RemoveListener(Change);
            source.onSelect.RemoveListener(Focus);
            source.onSelect.RemoveListener(Submit);
        }

        private void Blur(string value) => Blur();
        private void Change(string value) => Change();
        private void Focus(string value) => Focus();
        private void Submit(string value) => Submit();

        public void Blur()
        {
            SoundController.Play(blurEvent);
        }

        public void Change()
        {
            SoundController.Play(changeEvent);
        }

        public void Focus()
        {
            SoundController.Play(focusEvent);
        }

        public void Submit()
        {
            SoundController.Play(submitEvent);
        }
    }
}