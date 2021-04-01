using System.Linq;
using Essentials;
using UnityEngine;

namespace Tabs
{
    public class TabContainerController : MonoBehaviour
    {
        [SerializeField] private TabController[] _tabs = null;

        private TabCollectionState _state;

        public TabCollectionState State
        {
            get => _state;
            set => ApplyState(value);
        }

        public void Initialize()
        {
            for (var i = 0; i < _tabs.Length; i++)
            {
                var t = _tabs[i];
                t.Initialize(this);
                t.gameObject.SetActive(false);
            }
        }

        private void ApplyState(TabCollectionState state)
        {
            if (state.VisibleTabs != null)
            {
                var visibleTabs = state.VisibleTabs;
                var availableTabs = _tabs.Where(t => visibleTabs.Contains(t.Id)).ToArray();
                for (var i = 0; i < availableTabs.Length; i++)
                {
                    var tab = availableTabs[i];
                    tab.Header.gameObject.SetActive(true);
                }

                foreach (var t in _tabs.Except(availableTabs))
                {
                    t.gameObject.SetActive(false);
                    t.Header.gameObject.SetActive(false);
                }
            }

            Open(state.ActiveTab);
            _state = state;
        }

        public TabController GetTab(NamespacedKey tabId) => _tabs.FirstOrDefault(t => t.Id == tabId);

        public void Open(NamespacedKey tabId)
        {
            var tab = GetTab(tabId);
            Open(tab);
        }

        public void Open(TabController tab)
        {
            foreach (var t in _tabs.Where(t => t != tab && t.IsOpen))
            {
                t.Close();
            }

            tab.Open();
        }
    }
}