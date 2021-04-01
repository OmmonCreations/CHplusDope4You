using UnityEngine;

namespace Pagination
{
    public abstract class PaginatedViewEntryController : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform = null;
        
        private PaginatedViewEntry _entry;

        public PaginatedViewEntry Entry
        {
            get => _entry;
            set => ApplyEntry(value);
        }

        public RectTransform rectTransform => _rectTransform;

        #region Initializers
        
        public void Initialize()
        {
            OnInitialize();
        }
        
        #endregion
        
        #region Unity Control

        protected void OnDestroy()
        {
            OnDestroyed();
        }
        
        #endregion
        
        #region Actions

        public void Remove()
        {
            Destroy(gameObject);
            OnRemove();
        }
        
        #endregion

        #region Logic
        
        private void ApplyEntry(PaginatedViewEntry entry)
        {
            _entry = entry;
            Apply(entry);
        }

        #endregion
        
        #region Abstract Members

        protected abstract void Apply(PaginatedViewEntry entry);
        
        #endregion
        
        #region Virtual Members
        
        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnDestroyed()
        {
            
        }

        protected virtual void OnRemove()
        {
        }
        
        #endregion
    }

    public abstract class PaginatedViewEntryController<T> : PaginatedViewEntryController where T : PaginatedViewEntry
    {
        private T _entry;

        public new T Entry
        {
            get => _entry;
            set => base.Entry = value;
        }

        protected sealed override void Apply(PaginatedViewEntry entry)
        {
            if (!(entry is T t)) return;
            _entry = t;
            Apply(t);
        }

        protected abstract void Apply(T entry);
    }
}