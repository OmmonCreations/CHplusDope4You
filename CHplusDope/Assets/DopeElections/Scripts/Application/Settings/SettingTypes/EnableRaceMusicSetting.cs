using AppSettings;
using DopeElections.Sounds;
using FMODSoundInterface;

namespace DopeElections
{
    public class EnableRaceMusicSetting : BooleanSetting
    {
        public EnableRaceMusicSetting() : base("enable_race_music", true)
        {
            
        }

        protected override SettingValue<bool> CreateValueInstance()
        {
            var result = base.CreateValueInstance();
            result.ValueChanged += OnValueChanged;
            return result;
        }

        private void OnValueChanged(bool value)
        {
            SoundController.SetVolume(SoundCategory.RaceMusic, value ? 1 : 0);
        }
    }
}