using UnityEngine;
using UnityEngine.UI;

namespace FMODSoundInterface.UnityUI
{
    public class SliderSounds : AbstractSoundEmitter<Slider>
    {
        public string slideEvent = UISound.Slider.Slide;
        
        [Header("Auto-Link (optional)")]
        [SerializeField] private Slider _source = null;

        protected override Slider Source => _source;

        public override void HookEvents(Slider source)
        {
            source.onValueChanged.AddListener(Slide);
        }

        public override void ReleaseHooks(Slider source)
        {
            source.onValueChanged.RemoveListener(Slide);
        }

        private void Slide(float value) => Slide();

        public void Slide()
        {
            SoundController.Play(slideEvent);
        }
    }
}