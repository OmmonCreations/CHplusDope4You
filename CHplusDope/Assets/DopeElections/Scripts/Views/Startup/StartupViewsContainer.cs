using UnityEngine;
using Views;

namespace DopeElections.Startup
{
    public class StartupViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private EulaView _eulaView = null;
        [SerializeField] private LoadingScreenView _loadingScreenView = null;

        public EulaView Eula => _eulaView;
        public LoadingScreenView LoadingScreen => _loadingScreenView;
        
        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(Eula),
                new NavigationBranch(LoadingScreen)
            );
        }

    }
}