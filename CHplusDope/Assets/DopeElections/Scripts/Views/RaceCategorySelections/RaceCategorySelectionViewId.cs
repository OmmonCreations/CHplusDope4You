using Essentials;

namespace DopeElections.RaceCategorySelections
{
    public static class RaceCategorySelectionViewId
    {
        private const string Namespace = "category_selection";
        
        public static readonly NamespacedKey RandomSelection = new NamespacedKey(Namespace, "random_selection");
        public static readonly NamespacedKey FullSelection = new NamespacedKey(Namespace, "full_selection");
    }
}