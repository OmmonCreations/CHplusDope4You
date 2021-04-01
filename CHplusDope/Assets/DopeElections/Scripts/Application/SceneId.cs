using AppManagement;
using Essentials;

namespace DopeElections
{
    public static class SceneId
    {
        private const string Namespace = DopeElectionsApp.Namespace;
    
        public static readonly NamespacedKey Splash = new NamespacedKey(Namespace, "splash");
        public static readonly NamespacedKey Startup = new NamespacedKey(Namespace, "startup");
        public static readonly NamespacedKey Account = new NamespacedKey(Namespace, "account");
        public static readonly NamespacedKey MainMenu = new NamespacedKey(Namespace, "main_menu");
        public static readonly NamespacedKey Progress = new NamespacedKey(Namespace, "progress");
        public static readonly NamespacedKey RaceCategorySelection = new NamespacedKey(Namespace, "race_category_selection");
        public static readonly NamespacedKey Race = new NamespacedKey(Namespace, "race");
        public static readonly NamespacedKey RaceResult = new NamespacedKey(Namespace, "race_result");
        public static readonly NamespacedKey Scan = new NamespacedKey(Namespace, "scan");
    }
}