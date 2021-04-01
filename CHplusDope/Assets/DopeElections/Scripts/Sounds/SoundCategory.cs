namespace DopeElections.Sounds
{
    public static class SoundCategory
    {
        public static readonly string[] PausableCategories = {SoundCategory.Ambience, SoundCategory.Sfx};

        public const string Master = "bus:/master";
        
        public const string Music = "bus:/master/music";
        public const string Ambience = "bus:/master/ambience";
        public const string Sfx = "bus:/master/sfx";
        public const string UI = "bus:/master/ui";
        
        public const string RaceMusic = "bus:/master/music/race_music";
    }
}