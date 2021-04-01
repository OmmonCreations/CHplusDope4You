using System;
using UnityEngine;
using UnityEngine.UI;

namespace SortableCollections
{
    public abstract class FilterController<T> : MonoBehaviour where T : IFilter
    {

        public SortableCollection Collection { get; private set; }
        public T Filter { get; private set; }

        protected void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        public void Initialize(SortableCollection collection, T filter)
        {
            Collection = collection;
            Filter = filter;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }
    }
}