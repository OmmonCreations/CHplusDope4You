using Localizator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tabs
{
    public class TabHeaderController : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private Image _backgroundImage = null;
        [SerializeField] private Image _backgroundActiveImage = null;
        [SerializeField] private LocalizedText _labelText = null;
        
        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public TabContainerController TabContainer => Tab.TabContainer;
        public TabController Tab { get; private set; }

        public LocalizedText Label => _labelText;
        
        internal void Initialize(TabController tab)
        {
            Tab = tab;
            _button.onClick.AddListener(OnClick);
            tab.Opened += OnTabOpened;
            tab.Closed += OnTabClosed;
            UpdateState();
        }

        private void OnClick()
        {
            TabContainer.Open(Tab);
        }

        private void OnTabOpened()
        {
            UpdateState();
        }

        private void OnTabClosed()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if(_backgroundImage) _backgroundImage.gameObject.SetActive(!Tab.IsOpen);
            if(_backgroundActiveImage) _backgroundActiveImage.gameObject.SetActive(Tab.IsOpen);
        }
    }
}