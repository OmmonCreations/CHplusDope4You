using System.Linq;

namespace SortableCollections
{
    public class SortingState
    {
        public CollectionModifierState[] ModifierStates { get; }

        internal SortingState(SortableCollection collection) : this(collection.Modifiers.Select(f => f.GetCurrentState())
            .ToArray())
        {
        }

        public SortingState(params CollectionModifierState[] modifierStates)
        {
            ModifierStates = modifierStates;
        }
    }
}