using DopeElections.Races;
using Views;

namespace DopeElections.RaceCategorySelections
{
    public abstract class RaceCategorySelectionView : DopeElectionsView, IView<RaceContext>
    {
        public RaceCategorySelectionViewsContainer Views { get; private set; }
        protected RaceContext Context { get; private set; }

        protected override void OnInitialize(ViewsContainer viewsContainer)
        {
            Views = viewsContainer as RaceCategorySelectionViewsContainer;
        }

        public void Open(RaceContext context)
        {
            Context = context;
            base.Open();
        }
    }
}