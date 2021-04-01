using DopeElections.MainMenus.Empty;
using DopeElections.MainMenus.EndingCinematic;
using DopeElections.MainMenus.Final;
using DopeElections.Sounds;
using FMODSoundInterface;
using Localizator;
using UnityEngine;
using Views;

namespace DopeElections.MainMenus
{
    public class MainMenuViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private MenuBackground _menuBackground = null;
        [Header("Menus")] [SerializeField] private OverviewView _overviewView = null;
        [SerializeField] private InformationsView _informationsView = null;
        [SerializeField] private VoteInfosView _voteInfosView = null;
        [SerializeField] private LandSequenceView _landSequenceView = null;
        [SerializeField] private EndingCinematicView _endingCinematicView = null;
        [SerializeField] private EndingCreditsView _endingCreditsView = null;
        [SerializeField] private FinalView _finalView = null;

        public OverviewView OverviewView => _overviewView;
        public InformationsView InformationsView => _informationsView;
        public VoteInfosView VoteInfosView => _voteInfosView;
        public EndingCinematicView EndingCinematicView => _endingCinematicView;
        public EndingCreditsView EndingCreditsView => _endingCreditsView;
        public FinalView FinalView => _finalView;
        public LandSequenceView LandSequenceView => _landSequenceView;

        public MenuBackground Background => _menuBackground;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            BlackMask.FadeToClear();
        }

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(OverviewView,
                    new NavigationBranch(VoteInfosView),
                    new NavigationBranch(InformationsView)
                ),
                new NavigationBranch(LandSequenceView),
                new NavigationBranch(EndingCinematicView, 
                    new NavigationBranch(EndingCreditsView),
                    new NavigationBranch(FinalView)
                )
            );
        }
    }
}