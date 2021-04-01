using DopeElections.Answer;
using DopeElections.CandidateParties;
using DopeElections.Localizations;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class ElectionInfoController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _partyText = null;
        [SerializeField] private ListPlacesController _listPlaces = null;
        [SerializeField] private Image _partyImage = null;

        private Candidate _candidate = null;

        public Candidate Candidate
        {
            get => _candidate;
            set => ApplyCandidate(value);
        }

        private void ApplyCandidate(Candidate candidate)
        {
            var party = candidate.GetParty();
            _candidate = candidate;
            _partyText.key = party != null
                ? new LocalizationKey {fallback = party.abbr}
                : LKey.Components.Candidate.Party.None;
            _listPlaces.Candidate = candidate;

            if (party != null)
            {
                party.GetLogo().Assign(s =>
                {
                    if (!_partyImage) return;
                    _partyImage.sprite = s;
                    _partyImage.enabled = s;
                });
            }
            else
            {
                _partyImage.enabled = false;
            }
        }
    }
}