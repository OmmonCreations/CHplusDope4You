using System.Linq;
using UnityEngine;

namespace SortableCollections
{
    public abstract class SortableCollection
    {
        public delegate void CollectionEvent();

        public event CollectionEvent Updated = delegate { };

        public CollectionModifier[] Modifiers { get; }

        private string _sortingOrder;
        private bool _dirty = true;

        public string SortingOrder
        {
            get => _sortingOrder;
            set => ApplySortingOrder(value);
        }

        public abstract int Count { get; }
        public abstract int FilteredCount { get; }

        public SortingState SortState
        {
            get => new SortingState(this);
            set => ApplyState(value);
        }

        protected SortableCollection(CollectionModifier[] modifiers)
        {
            Modifiers = modifiers;
            foreach (var m in modifiers)
            {
                m.Changed += SetDirty;
            }
        }

        private void ApplyState(SortingState state)
        {
            foreach (var m in Modifiers)
            {
                m.LoadState(state);
            }
        }

        protected virtual void ApplySortingOrder(string id)
        {
            _sortingOrder = id;
            SetDirty();
        }

        public void SetDirty()
        {
            _dirty = true;
        }

        public void Update()
        {
            if (!_dirty) return;
            UpdateEntries();
            _dirty = false;
            Updated();
        }

        protected abstract void UpdateEntries();
    }

    public abstract class SortableCollection<T> : SortableCollection
    {
        private T[] _filteredEntries;
        public T[] AllEntries { get; }

        public T[] FilteredEntries
        {
            get
            {
                Update();
                return _filteredEntries;
            }
            private set => _filteredEntries = value;
        }

        public override int Count => AllEntries.Length;
        public override int FilteredCount => FilteredEntries.Length;

        protected SortableCollection(T[] entries, params CollectionModifier[] modifiers) : base(modifiers)
        {
            AllEntries = entries;
        }

        protected sealed override void ApplySortingOrder(string id)
        {
            base.ApplySortingOrder(id);
            SetDirty();
        }

        protected override void UpdateEntries()
        {
            var filters = Modifiers.OfType<IFilter<T>>().Where(f => f.Active);
            var sortingOrders = Modifiers.OfType<ISortingOrder<T>>().ToArray();
            var sortingOrder = sortingOrders.FirstOrDefault(o => o.Id == SortingOrder) ??
                               sortingOrders.FirstOrDefault();
            var filtered = filters.Aggregate(AllEntries, (current, f) => f.Apply(current).ToArray());
            FilteredEntries = sortingOrder != null ? sortingOrder.Apply(filtered).ToArray() : filtered;
        }
    }
}