using System.Collections.Generic;
using Localizator;

namespace SortableCollections
{
    public interface ISortingOrder
    {
        string Id { get; }
        LocalizationKey Label { get; }
        bool Reverse { get; set; }
    }
    
    public interface ISortingOrder<T> : ISortingOrder
    {
        IEnumerable<T> Apply(IEnumerable<T> entries);
    }
}