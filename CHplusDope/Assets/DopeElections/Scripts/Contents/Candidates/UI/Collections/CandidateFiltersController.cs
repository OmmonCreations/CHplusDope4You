using System.Linq;
using DopeElections.Localizations;
using Localizator;
using SortableCollections;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class CandidateFiltersController : MonoBehaviour
    {
        [SerializeField] private GameObject _filtersLayer = null;
        [SerializeField] private Button _closeBackground = null; 
        [SerializeField] private Button _confirmButton = null;
        [SerializeField] private LocalizedText _confirmText = null;
        [SerializeField] private PartyFilterController _partyFilter = null;
        [SerializeField] private SortingButtonController _partySortingButton = null;
        [SerializeField] private SortingButtonController _ageSortingButton = null;
        [SerializeField] private SortingButtonController _matchSortingButton = null;

        private CandidateCollection _collection;
        
        public CandidateCollection Collection
        {
            get => _collection;
            set => ApplyCollection(value);
        }

        private void Awake()
        {
            _closeBackground.onClick.AddListener(Close);
            _confirmButton.onClick.AddListener(Close);
            _confirmText.key = LKey.Components.CandidateCollection.Filters.Confirm;
        }

        public void Open() => Open(true);

        public void Close() => Open(false);

        private void Open(bool open)
        {
            _filtersLayer.SetActive(open);
        }

        public void OpenImmediately() => OpenImmediately(true);
        public void CloseImmediately() => OpenImmediately(false);

        private void OpenImmediately(bool open)
        {
            _filtersLayer.SetActive(open);
        }

        private void ApplyCollection(CandidateCollection collection)
        {
            _collection = collection;
            _partyFilter.Initialize(collection, collection.Modifiers.OfType<PartyFilter>().FirstOrDefault());
            _partySortingButton.Initialize(collection,
                collection.Modifiers.OfType<PartySortingOrder>().FirstOrDefault());
            _ageSortingButton.Initialize(collection, collection.Modifiers.OfType<AgeSortingOrder>().FirstOrDefault());
            _matchSortingButton.Initialize(collection,
                collection.Modifiers.OfType<MatchSortingOrder>().FirstOrDefault());
        }
    }
}