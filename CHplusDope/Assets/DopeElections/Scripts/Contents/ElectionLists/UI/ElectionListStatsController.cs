using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DopeElections.Answer;
using DopeElections.Candidates;
using DopeElections.Localizations;
using Localizator;
using TMPro;
using UnityEngine;

namespace DopeElections.ElectionLists.UI
{
    public class ElectionListStatsController : MonoBehaviour
    {
        [Header("All fields are optional.")] [SerializeField]
        private LocalizedText _seatsLabelText = null;

        [SerializeField] private TMP_Text _seatsValueText = null;
        [SerializeField] private LocalizedText _ageLabelText = null;
        [SerializeField] private TMP_Text _ageValueText = null;
        [SerializeField] private TMP_Text _femalePercentageText = null;
        [SerializeField] private TMP_Text _malePercentageText = null;
        [SerializeField] private TMP_Text _partyDistributionText = null;
        [SerializeField] private RectTransform _partyDistributionListArea = null;
        [SerializeField] private PartyDistributionEntry _partyDistributionTemplate = null;

        public Election Election { get; private set; }
        public ElectionList List { get; private set; }

        private KeyValuePair<Party, float>[] _partyDistribution;
        private PartyDistributionEntry[] _partyDistributionEntries = null;

        private void Awake()
        {
            if (_seatsLabelText) _seatsLabelText.key = LKey.Components.ElectionList.Seats.Label;
            if (_ageLabelText) _ageLabelText.key = LKey.Components.ElectionList.Age.Label;
            if (_partyDistributionTemplate) _partyDistributionTemplate.gameObject.SetActive(false);
        }

        public void SetElectionList(Election election, ElectionList list)
        {
            Election = election;
            List = list;
            UpdateState();
        }

        public void UpdateState()
        {
            var election = Election;
            var list = List;

            var seats = election.seats;
            var candidates = list != null ? list.GetCandidates() : new Candidate[0];

            var candidateCount = candidates.Length;

            var parties = candidates.GetParties();
            if(parties.Contains(null)) Debug.LogError("There is a null party!");
            var partyDistribution = parties.ToDictionary(
                p => p,
                p => candidates.Count(c => c.partyId == p.id) / (float) candidateCount
            );
            var totalPartyDistribution = partyDistribution.Sum(e => e.Value);
            if (totalPartyDistribution < 1)
            {
                partyDistribution[new Party() {abbr = "no party"}] = 1 - totalPartyDistribution;
            }

            _partyDistribution = partyDistribution
                .Select(e => new KeyValuePair<Party, float>(e.Key, e.Value))
                .OrderBy(e => e.Value)
                .ToArray();

            if (_seatsValueText) _seatsValueText.text = $"{candidateCount}/{seats}";

            if (_ageValueText)
            {
                var averageAge = candidates.GetAverageAge();
                _ageValueText.text = averageAge > 0 ? averageAge.ToString(CultureInfo.InvariantCulture) : "-";
            }

            if (_malePercentageText)
            {
                var maleRatio = candidates.GetMaleRatio();
                _malePercentageText.text = !float.IsNaN(maleRatio)
                    ? Mathf.RoundToInt(maleRatio * 100) + "%"
                    : "-";
            }

            if (_femalePercentageText)
            {
                var femaleRatio = candidates.GetFemaleRatio();
                _femalePercentageText.text = !float.IsNaN(femaleRatio)
                    ? Mathf.RoundToInt(femaleRatio * 100) + "%"
                    : "-";
            }

            UpdatePartyDistribution();
        }

        private void UpdatePartyDistribution()
        {
            var partyDistribution = _partyDistribution;

            ClearPartyDistribution();

            if (_partyDistributionText)
            {
                _partyDistributionText.text = string.Join(", ", partyDistribution
                    .Take(Mathf.Min(partyDistribution.Length, 3))
                    .Select(e => Mathf.RoundToInt(e.Value * 100) + "% " + e.Key.abbr));
            }

            if (_partyDistributionListArea && _partyDistributionTemplate)
            {
                _partyDistributionEntries = CreatePartyDistribution(partyDistribution);
            }
        }

        private void ClearPartyDistribution()
        {
            if (_partyDistributionEntries == null) return;
            foreach (var e in _partyDistributionEntries) e.Remove();
        }

        private PartyDistributionEntry[] CreatePartyDistribution(KeyValuePair<Party, float>[] entries)
        {
            return entries.Select(e => CreatePartyDistributionEntry(e.Key, e.Value)).ToArray();
        }

        private PartyDistributionEntry CreatePartyDistributionEntry(Party party, float percentage)
        {
            var instanceObject = Instantiate(_partyDistributionTemplate.gameObject, _partyDistributionListArea, false);
            var instance = instanceObject.GetComponent<PartyDistributionEntry>();
            instance.Set(party, percentage);
            instanceObject.SetActive(true);
            return instance;
        }
    }
}