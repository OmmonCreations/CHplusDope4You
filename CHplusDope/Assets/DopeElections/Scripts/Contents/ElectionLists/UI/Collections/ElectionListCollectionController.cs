using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using UnityEngine;

namespace DopeElections.ElectionLists.UI.Collections
{
    /// <summary>
    /// Represents a sortable view of election lists
    /// </summary>
    public class ElectionListCollectionController : MonoBehaviour
    {
        [SerializeField] private RectTransform _entriesArea = null;
        [SerializeField] private ElectionListEntryController _entryPrefab = null;

        private ElectionListCollection _collection;
        private readonly List<ElectionListEntryController> _entries = new List<ElectionListEntryController>();

        public ElectionListCollection Collection
        {
            get => _collection;
            set => ApplyCollection(value);
        }

        private void Update()
        {
            if (Collection != null) Collection.Update();
        }

        private void ApplyCollection(ElectionListCollection collection)
        {
            _collection = collection;
            collection.Updated += OnCollectionUpdated;
            collection.Update();
            OnCollectionUpdated();
            //_sections.AddRange(CreateSections(collection.Sections));
        }

        private void OnCollectionUpdated()
        {
            var entriesAreaObject = _entriesArea.gameObject;
            var areaWasActive = entriesAreaObject.activeSelf;
            if(areaWasActive) entriesAreaObject.SetActive(false);
            ClearEntries();
            var election = Collection.Election;
            var entries = Collection.FilteredEntries;
            _entries.AddRange(CreateEntries(election, entries));
            if(areaWasActive) entriesAreaObject.SetActive(true);
        }

        private void ClearEntries()
        {
            foreach (var s in _entries)
            {
                s.Remove();
            }

            _entries.Clear();
        }

        private IEnumerable<ElectionListEntryController> CreateEntries(Election election, IEnumerable<ElectionList> entries)
        {
            return entries.Select(e=>CreateEntry(election, e));
        }

        private ElectionListEntryController CreateEntry(Election election, ElectionList list)
        {
            var instanceObject = Instantiate(_entryPrefab.gameObject, _entriesArea, false);
            var instance = instanceObject.GetComponent<ElectionListEntryController>();
            instance.SetElectionList(election, list);
            return instance;
        }
    }
}