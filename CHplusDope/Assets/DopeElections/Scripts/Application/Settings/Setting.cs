using DopeElections.Sounds;
using FMODSoundInterface.Settings;
using Localizator.Settings;

namespace DopeElections
{
    public static class Setting
    {
        public static readonly EulaSetting EulaAccepted
            = new EulaSetting();

        public static readonly LanguageSetting Language
            = new LanguageSetting("language",
                Localizator.Language.german,
                Localizator.Language.french
            );

        public static readonly DeviceIdSetting DeviceId
            = new DeviceIdSetting("device_id");

        public static readonly EnableRaceMusicSetting EnableRaceMusic
            = new EnableRaceMusicSetting();

        public static readonly AudioSetting MasterVolume
            = new AudioSetting("volume_master", SoundCategory.Master);

        public static readonly AudioSetting SfxVolume
            = new AudioSetting("volume_sfx", SoundCategory.Sfx);

        public static readonly AudioSetting UIVolume
            = new AudioSetting("volume_ui", SoundCategory.UI);

        public static readonly AudioSetting MusicVolume
            = new AudioSetting("volume_music", SoundCategory.Music);

        public static readonly AudioSetting AmbienceVolume
            = new AudioSetting("volume_ambience", SoundCategory.Ambience);
    }
}