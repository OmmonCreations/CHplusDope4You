using System.Linq;

namespace Views
{
    public class NavigationTree : INavigationLayer
    {
        public NavigationBranch[] Branches { get; }
        public View[] Views { get; }

        public NavigationTree(params NavigationBranch[] branches)
        {
            Branches = branches;
            Views = GetAllViews(this);
        }

        public NavigationBranch[] GetAllBranches()
        {
            return GetAllBranches(this);
        }

        private NavigationBranch[] GetAllBranches(INavigationLayer layer)
        {
            return layer.Branches.SelectMany(GetAllBranches).ToArray();
        }

        private View[] GetAllViews(INavigationLayer layer)
        {
            return layer.Branches.SelectMany(l => GetAllViews(l).Append(l.View)).Distinct().ToArray();
        }
    }
}