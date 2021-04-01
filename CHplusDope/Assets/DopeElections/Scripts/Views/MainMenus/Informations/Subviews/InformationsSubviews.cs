using UnityEngine;
using Views;

namespace DopeElections.MainMenus
{
    public class InformationsSubviews : DopeElectionsViewsContainer
    {
        [SerializeField] private HowToPlayView _howToPlayView = null;
        [SerializeField] private CreditsView _creditsView = null;

        public HowToPlayView HowToPlayView => _howToPlayView;
        public CreditsView CreditsView => _creditsView;

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(HowToPlayView),
                new NavigationBranch(CreditsView)
            );
        }
    }
}