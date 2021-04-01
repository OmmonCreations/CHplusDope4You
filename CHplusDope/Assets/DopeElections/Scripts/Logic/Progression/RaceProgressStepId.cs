using Essentials;

namespace DopeElections.Progression
{
    public static class RaceProgressStepId
    {
        private const string Namespace = DopeElectionsApp.Namespace;
        
        public static NamespacedKey TranslationCheck { get; } = new NamespacedKey(Namespace, "translation_check");
        public static NamespacedKey ExtraInfo1 { get; } = new NamespacedKey(Namespace, "extra_info_1");
        public static NamespacedKey ExtraInfo2 { get; } = new NamespacedKey(Namespace, "extra_info_2");
        public static NamespacedKey ExtraInfo3 { get; } = new NamespacedKey(Namespace, "extra_info_3");
        public static NamespacedKey ExtraInfo4 { get; } = new NamespacedKey(Namespace, "extra_info_4");
        public static NamespacedKey EndingComic { get; } = new NamespacedKey(Namespace, "ending_comic");
    }
}