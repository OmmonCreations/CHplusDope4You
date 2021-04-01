using AppSettings;

namespace FMODSoundInterface.Settings
{
    public class AudioSetting : SettingType<float>
    {
        public string Category { get; }
        public override float DefaultValue { get; }
    
        public AudioSetting(string key, string category, float defaultValue = 0.5f) : base(key)
        {
            Category = category;
            DefaultValue = defaultValue;
        }

        protected override SettingValue<float> CreateValueInstance()
        {
            var result = new FloatValue();
            result.ValueChanged += OnValueChanged;
            return result;
        }

        private void OnValueChanged(float value)
        {
            SoundController.SetVolume(Category, value);
        }
    }
}
