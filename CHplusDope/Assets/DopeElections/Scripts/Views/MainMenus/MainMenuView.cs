using Views;

namespace DopeElections.MainMenus
{
    public abstract class MainMenuView : DopeElectionsView, IView
    {
        public MainMenuViewsContainer Views { get; private set; }
        
        protected override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as MainMenuViewsContainer;
        }

        public new void Open()
        {
            base.Open();
        }
    }
}