using Essentials;

namespace DopeElections.Progression
{
    public static class UserJourneyStepId
    {
        private const string Namespace = DopeElectionsApp.Namespace;

        public static NamespacedKey CharacterCreation { get; } = new NamespacedKey(Namespace, "character_creation");
        public static NamespacedKey GrabSequence { get; } = new NamespacedKey(Namespace, CutsceneId.GrabSequence);
        public static NamespacedKey OpeningComic { get; } = new NamespacedKey(Namespace, CutsceneId.OpeningComic);
        public static NamespacedKey MainMenuLanding { get; } = new NamespacedKey(Namespace, CutsceneId.MainMenuLanding);
        public static NamespacedKey FirstRace { get; } = new NamespacedKey(Namespace, "first_race");
        public static NamespacedKey RaceInfo { get; } = new NamespacedKey(Namespace, "race_info");
    }
}