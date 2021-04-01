using Essentials;

namespace DopeElections.Startup
{
    public static class StartupViewId
    {
        private const string Namespace = "startup";

        public static readonly NamespacedKey LoadingScreen = new NamespacedKey(Namespace, "loading_screen");
        public static readonly NamespacedKey UserConsent = new NamespacedKey(Namespace, "user_consent");
    }
}