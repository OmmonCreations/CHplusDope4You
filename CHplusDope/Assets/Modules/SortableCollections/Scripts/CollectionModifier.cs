using System.Collections.Generic;
using System.Linq;

namespace SortableCollections
{
    public abstract class CollectionModifier
    {
        public delegate void ModifierEvent();
        public event ModifierEvent Changed = delegate { };
        
        internal abstract CollectionModifierState GetCurrentState();
        internal abstract void LoadState(SortingState sortState);

        protected void TriggerChanged()
        {
            Changed();
        }
    }
    
    public abstract class CollectionModifier<T2> : CollectionModifier where T2 : CollectionModifierState
    {
        private T2 _state;

        public T2 State
        {
            get => _state ?? DefaultState;
            set => ApplyState(value);
        }

        protected abstract T2 DefaultState { get; }

        internal override CollectionModifierState GetCurrentState()
        {
            return State;
        }

        private void ApplyState(T2 state)
        {
            _state = state;
            TriggerChanged();
        }

        internal override void LoadState(SortingState sortState)
        {
            State = sortState.ModifierStates.OfType<T2>().DefaultIfEmpty(DefaultState).FirstOrDefault();
        }
    }
}