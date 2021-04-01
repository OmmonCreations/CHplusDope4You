using DopeElections.Races;
using Views;

namespace DopeElections.RaceResults
{
    public abstract class RaceResultView : DopeElectionsView, IView<RaceContext>
    {
        public RaceResultViewsContainer Views { get; private set; }
        protected RaceContext Context { get; private set; }
        
        protected sealed override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as RaceResultViewsContainer;
        }

        public void Open(RaceContext context)
        {
            Context = context;
            base.Open();
        }
    }
}