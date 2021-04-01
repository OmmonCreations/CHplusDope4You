using System;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace SortableCollections
{
    public class SortingButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        // [SerializeField] private LocalizedText _labelText = null;
        [SerializeField] private Image _normalImage = null;
        [SerializeField] private Image _reverseImage = null;
        [SerializeField] private GameObject _activeState = null;

        public SortableCollection Collection { get; private set; }
        public ISortingOrder SortingOrder { get; private set; }

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Initialize(SortableCollection collection, ISortingOrder filter)
        {
            Collection = collection;
            SortingOrder = filter;
            collection.Updated += UpdateState;
            // _labelText.key = filter.Label;
            UpdateState();
        }

        private void OnClick()
        {
            if (Collection.SortingOrder == SortingOrder.Id)
            {
                if (SortingOrder.Reverse)
                {
                    Collection.SortingOrder = null;
                }
                else
                {
                    SortingOrder.Reverse = true;
                }
            }
            else
            {
                SortingOrder.Reverse = false;
                Collection.SortingOrder = SortingOrder.Id;
            }
            
            Collection.Update();
        }

        private void UpdateState()
        {
            var enabled = Collection.SortingOrder == SortingOrder.Id;
            _activeState.SetActive(enabled);
            _normalImage.gameObject.SetActive(enabled && !SortingOrder.Reverse);
            _reverseImage.gameObject.SetActive(enabled && SortingOrder.Reverse);
        }
    }
}