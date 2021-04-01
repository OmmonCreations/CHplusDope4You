using UnityEngine;
using Views;

namespace DopeElections.Splash
{
    public class SplashViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private SplashView _splashView = null;

        public SplashView SplashView => _splashView;
    
        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(new NavigationBranch(SplashView));
        }
    }
}