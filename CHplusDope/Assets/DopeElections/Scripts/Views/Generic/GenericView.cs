using Views;

namespace DopeElections
{
    public abstract class GenericView : DopeElectionsView
    {
        public GenericViewsContainer Views { get; private set; }

        protected sealed override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as GenericViewsContainer;
            base.OnInitialize(viewsContainer);
        }
    }
}