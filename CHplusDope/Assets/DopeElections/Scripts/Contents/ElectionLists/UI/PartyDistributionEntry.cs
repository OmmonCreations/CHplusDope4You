using DopeElections.Answer;
using DopeElections.CandidateParties;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.ElectionLists.UI
{
    public class PartyDistributionEntry : MonoBehaviour
    {
        [Header("All fields are optional.")]
        [SerializeField] private Image _logoImage = null;
        [SerializeField] private TMP_Text _abbreviationText = null;
        [SerializeField] private TMP_Text _percentageText = null;

        public void Set(Party party, float percentage)
        {
            if(_logoImage) party.GetLogo().Assign(s=>
            {
                if(_logoImage) _logoImage.sprite = s;
            });
            if(_abbreviationText) _abbreviationText.text = party.abbr.ToUpper();
            if(_percentageText) _percentageText.text = Mathf.RoundToInt(percentage * 100) + "%";
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}