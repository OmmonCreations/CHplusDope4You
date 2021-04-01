using Views;

namespace DopeElections.Progress
{
    public abstract class ProgressView : DopeElectionsView
    {
        public ProgressViewsContainer Views { get; private set; }

        protected override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as ProgressViewsContainer;
        }
    }
}