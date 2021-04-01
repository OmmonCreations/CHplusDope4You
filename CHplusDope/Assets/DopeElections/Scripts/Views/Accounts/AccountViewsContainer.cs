using UnityEngine;
using Views;

namespace DopeElections.Accounts
{
    public class AccountViewsContainer : DopeElectionsViewsContainer
    {
        [SerializeField] private SettingsView _settingsView = null;
        [SerializeField] private FaceSelectionView _faceSelectionView = null;
        [SerializeField] private LocationSelectionView _locationSelectionView = null;
        [SerializeField] private ComicSequenceView _comicSequenceView = null;

        public SettingsView SettingsView => _settingsView;
        public FaceSelectionView FaceSelectionView => _faceSelectionView;
        public LocationSelectionView LocationSelectionView => _locationSelectionView;
        public ComicSequenceView ComicSequenceView => _comicSequenceView;

        public AccountSceneController EditorController { get; private set; }

        public void Initialize(AccountSceneController editorController)
        {
            EditorController = editorController;
            Initialize();
        }

        protected override NavigationTree BuildNavigationTree()
        {
            return new NavigationTree(
                new NavigationBranch(SettingsView),
                new NavigationBranch(FaceSelectionView),
                new NavigationBranch(LocationSelectionView),
                new NavigationBranch(ComicSequenceView)
            );
        }
    }
}