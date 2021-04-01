using UnityEngine;
using Views;

namespace DopeElections.RaceCategorySelections
{
    public class RaceCategorySelectionViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private RandomSelectionView _randomSelectionView = null;

        public RandomSelectionView RandomSelectionView => _randomSelectionView;

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(_randomSelectionView)
            );
        }
    }
}