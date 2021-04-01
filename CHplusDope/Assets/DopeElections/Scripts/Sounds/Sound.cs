namespace DopeElections.Sounds
{
    public static class Sound
    {
        public static class Sfx
        {
            public static class Character
            {
                public const string Talk = "event:/sfx/character/talk";
                public const string Fall = "event:/sfx/character/fall";
                public const string Jump = "event:/sfx/character/jump";
                public const string YellHappy = "event:/sfx/character/yell-happy";
                public const string YellScared = "event:/sfx/character/yell-scared";
                public const string Confused = "event:/sfx/character/confused";
                public const string CrowdCheer = "event:/sfx/character/crowd-cheer";
                public const string Squish = "event:/sfx/character/squish";
                public const string Boost = "event:/sfx/character/boost";
            }

            public static class Player
            {
                public const string FaceChanged = "event:/sfx/player/face-changed";
            }

            public static class Cow
            {
                public const string Talk = "event:/sfx/cow/talk";
            }

            public static class Ibex
            {
                public const string Talk = "event:/sfx/ibex/talk";
            }

            public static class Murmeli
            {
                public const string Talk = "event:/sfx/murmeli/talk";
            }

            public static class AppleWorm
            {
                public const string Appear = "event:/sfx/apple-worm/talk";
                public const string Talk = "event:/sfx/apple-worm/talk";
            }

            public static class Particle
            {
                public const string ExplosionConfetti = "event:/sfx/particle/explosion-confetti";
            }

            public static class Special
            {
                public const string RaceComplete = "event:/sfx/special/race_complete";
                public const string Congratulations = "event:/sfx/special/congratulations";
            }
        }

        public static class Ambience
        {
            public const string DeepSpace = "event:/ambience/deep-space";
            public const string CrowdCheer = "event:/ambience/crowd-cheer";
        }

        public static class UI
        {
            public static class Button
            {
                public const string Tap = "event:/ui/button/tap";
                public const string Confirm = "event:/ui/button/confirm";
                public const string Cancel = "event:/ui/button/cancel";
            }
        }
    }
}