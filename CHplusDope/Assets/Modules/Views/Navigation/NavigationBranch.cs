namespace Views
{
    public class NavigationBranch : INavigationLayer
    {
        public View View { get; }
        public NavigationBranch[] Branches { get; }
        public BranchConfiguration Configuration { get; }

        public NavigationBranch(View view, params NavigationBranch[] branches) : this(view, BranchConfiguration.Default,
            branches)
        {

        }

        public NavigationBranch(View view, BranchConfiguration configuration, NavigationBranch[] branches = null)
        {
            View = view;
            Configuration = configuration;
            Branches = branches != null ? branches : new NavigationBranch[0];
        }
    }
}