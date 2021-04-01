using DopeElections.Answer;
using DopeElections.Localizations;
using DopeElections.Placeholders;
using DopeElections.SmartSpiders;
using Localizator;
using TMPro;
using UnityEngine;

namespace DopeElections.ElectionLists.UI
{
    public class ElectionListEntryController : MonoBehaviour
    {
        [SerializeField] private LocalizedText _listNumberText = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private LocalizedText _matchText = null;
        [SerializeField] private ElectionListStatsController _stats = null;
        [SerializeField] private MiniSmartSpider _smartSpider = null;
        
        private ElectionList _electionList;

        public ElectionList ElectionList => _electionList;

        private void Awake()
        {
            _matchText.key = LKey.Components.ElectionList.Match.Amount;
        }

        public void SetElectionList(Election election, ElectionList list)
        {
            var listMatch = list.GetMatchPercentage(DopeElectionsApp.Instance.User);
            _electionList = list;

            _nameText.text = list.name;

            _listNumberText.key = list.GetListNumberKey();
            _listNumberText.SetVariable("number", list.number.ToString());
            _matchText.SetVariable("value", !float.IsNaN(listMatch) 
                ? Mathf.RoundToInt(listMatch * 100).ToString() 
                : "-");
            _stats.SetElectionList(election, list);
            _smartSpider.Value = list.GetSmartSpider().Values;
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}