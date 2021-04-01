using System.Collections.Generic;
using Localizator;

namespace SortableCollections
{
    public interface IFilter
    {
        bool Active { get; }
        LocalizationKey Label { get; }
    }
    
    public interface IFilter<T> : IFilter
    {
        IEnumerable<T> Apply(IEnumerable<T> entries);
    }
}