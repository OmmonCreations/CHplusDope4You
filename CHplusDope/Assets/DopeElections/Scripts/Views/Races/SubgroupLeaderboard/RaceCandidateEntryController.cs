using DopeElections.Candidates;
using Pagination;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Races
{
    [AddComponentMenu("Subgroup Leaderboard Entry")]
    public class RaceCandidateEntryController : PaginatedViewEntryController<CandidateEntry>
    {
        [SerializeField] private CandidateEntryController _candidate = null;
        [SerializeField] private TMP_Text _matchText = null;
        [SerializeField] private Button _button = null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _button.onClick.AddListener(Select);
        }

        protected override void Apply(CandidateEntry entry)
        {
            _candidate.Candidate = entry.Candidate.Candidate;
            _matchText.text = Mathf.RoundToInt(entry.CategoryMatch * 100) + "%";
        }

        private void Select()
        {
            var action = Entry != null ? Entry.SelectAction : null;
            if (action == null) return;
            action();
        }
    }
}