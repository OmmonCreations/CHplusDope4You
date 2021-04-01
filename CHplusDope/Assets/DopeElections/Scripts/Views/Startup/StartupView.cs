using Views;

namespace DopeElections.Startup
{
    public abstract class StartupView : DopeElectionsView
    {
        public StartupViewsContainer Views { get; private set; }

        protected sealed override void OnInitialize(ViewsContainer viewsContainer)
        {
            base.OnInitialize(viewsContainer);
            Views = viewsContainer as StartupViewsContainer;
        }
    }
}