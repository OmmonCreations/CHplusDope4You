using Essentials;
using Localizator;
using UnityEngine;

namespace Tabs
{
    public class TabController : MonoBehaviour
    {
        public delegate void TabEvent();

        public event TabEvent Opened = delegate { };
        public event TabEvent Closed = delegate { };

        [SerializeField] private string _id = null;
        [SerializeField] private TabHeaderController _header = null;

        public NamespacedKey Id => NamespacedKey.TryParse(_id, out var id) ? id : default;
        internal TabContainerController TabContainer { get; private set; }
        internal TabHeaderController Header => _header;
        public bool IsOpen { get; private set; }
        
        internal void Initialize(TabContainerController container)
        {
            TabContainer = container;
            _header.Initialize(this);
        }

        public void Open(bool open)
        {
            if (IsOpen == open) return;
            IsOpen = open;
            gameObject.SetActive(open);
            if (open) Opened();
            else Closed();
        }

        public void Open() => Open(true);
        public void Close() => Open(false);
    }
}