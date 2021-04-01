using System.Collections.Generic;
using System.Linq;
using DopeElections.Answer;
using SortableCollections;
using UnityEngine;

namespace DopeElections.Candidates
{
    public class PartyFilterController : FilterController<PartyFilter>
    {
        [SerializeField] private RectTransform _partiesArea = null;
        [SerializeField] private PartyFilterEntryController _template = null;

        private PartyFilterEntryController[] _entries;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _template.gameObject.SetActive(false);
            CreateEntries();
        }

        private void OnPartyToggled()
        {
            Filter.State = new PartyFilter.FilterState(_entries
                .Where(e => e.Selected)
                .Select(e => e.Party)
                .ToArray());
        }

        private void CreateEntries()
        {
            var areaWasActive = _partiesArea.gameObject.activeSelf;
            ClearEntries();
            var selected = Filter.State.Parties;
            if (areaWasActive) _partiesArea.gameObject.SetActive(false);
            var collection = Collection as CandidateCollection;
            var partyIds = collection != null
                ? collection.AllEntries.Select(c => c.partyId).Distinct().ToList()
                : new List<int>();
            _entries = DopeElectionsApp.Instance.Assets.GetAssets<Party>()
                .Where(p => partyIds.Any(id => id == p.id))
                .Select(CreateEntry)
                .ToArray();
            foreach (var e in _entries)
            {
                e.SetValueWithoutNotify(selected.Any(p => p.id == e.Party.id));
            }

            if (areaWasActive) _partiesArea.gameObject.SetActive(true);
        }

        private void ClearEntries()
        {
            if (_entries == null) return;
            foreach (var e in _entries)
            {
                e.Remove();
            }
        }

        private PartyFilterEntryController CreateEntry(Party party)
        {
            var instanceObject = Instantiate(_template.gameObject, _partiesArea, false);
            var instance = instanceObject.GetComponent<PartyFilterEntryController>();
            instance.Initialize(party);
            instance.Changed += isOn => OnPartyToggled();
            instanceObject.SetActive(true);
            return instance;
        }
    }
}