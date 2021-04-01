using Essentials;

namespace DopeElections.Users
{
    public static class PlayerFaceId
    {
        private const string Namespace = DopeElectionsApp.Namespace;

        public static NamespacedKey Missing { get; } = new NamespacedKey(Namespace, "missing_face");
        public static NamespacedKey Frightened { get; } = new NamespacedKey(Namespace, "frightened_face");
        public static NamespacedKey Smile { get; } = new NamespacedKey(Namespace, "smile_face");
        public static NamespacedKey Happy { get; } = new NamespacedKey(Namespace, "happy_face");
        public static NamespacedKey Angry { get; } = new NamespacedKey(Namespace, "angry_face");
        public static NamespacedKey Exhausted { get; } = new NamespacedKey(Namespace, "exhausted_face");
    }
}