using Essentials;

namespace Tabs
{
    public class TabCollectionState
    {
        public NamespacedKey ActiveTab { get; }
        public NamespacedKey[] VisibleTabs { get; set; }

        public TabCollectionState(NamespacedKey activeTab)
        {
            ActiveTab = activeTab;
        }
    }
}