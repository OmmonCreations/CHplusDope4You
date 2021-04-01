using Views;

namespace DopeElections.Races
{
    public abstract class RaceView : DopeElectionsView
    {
        protected RaceViewsContainer Views { get; private set; }
        protected RaceController RaceController => Views.RaceController;

        protected sealed override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as RaceViewsContainer;
        }
    }
}