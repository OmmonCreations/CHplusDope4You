using DopeElections.MainMenus.Subviews.HowToVote;
using DopeElections.MainMenus.Subviews.OurSystem;
using UnityEngine;
using Views;

namespace DopeElections.MainMenus.Subviews
{
    public class VoteInfosSubviews : DopeElectionsViewsContainer
    {
        [SerializeField] private HowToVoteView _howToVoteView = null;
        [SerializeField] private OurSystemView _ourSystemView = null;

        public HowToVoteView HowToVoteView => _howToVoteView;
        public OurSystemView OurSystemView => _ourSystemView;

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(HowToVoteView),
                new NavigationBranch(OurSystemView)
            );
        }
    }
}