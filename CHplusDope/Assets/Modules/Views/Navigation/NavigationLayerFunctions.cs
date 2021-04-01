using System.Linq;

namespace Views
{
    public static class NavigationLayerFunctions
    {
        public static INavigationLayer GetLayer(this INavigationLayer layer, View view)
        {
            if (layer.Branches.Any(b => b.View == view)) return layer;
            return layer.Branches.FirstOrDefault(b => b.GetLayer(view) != null);
        }

        public static NavigationBranch GetBranch(this INavigationLayer layer, View view)
        {
            var currentLayer = layer.Branches.FirstOrDefault(b => b.View == view);
            if (currentLayer != null) return currentLayer;
            return layer.Branches.Select(b => b.GetBranch(view)).FirstOrDefault(b => b != null);
        }

        public static bool Contains(this INavigationLayer layer, View view)
        {
            if (layer.Branches.Any(b => b.View == view)) return true;
            return layer.Branches.Any(b => b.Contains(view));
        }

        public static bool Contains(this INavigationLayer layer, INavigationLayer other)
        {
            if (layer.Branches.Any(b => b==other)) return true;
            return layer.Branches.Any(b => b.Contains(other));
        }
    }
}