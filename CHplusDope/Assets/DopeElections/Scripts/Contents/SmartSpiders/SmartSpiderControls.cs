using DopeElections.Candidates;
using DopeElections.ElectionLists;
using DopeElections.Localizations;
using Localizator;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.SmartSpiders
{
    public class SmartSpiderControls : MonoBehaviour
    {
        [SerializeField] private Toggle _candidateLayerToggle = null;
        [SerializeField] private Toggle _userLayerToggle = null;
        [SerializeField] private Toggle _previousUserLayerToggle = null;
        [SerializeField] private Toggle _listLayerToggle = null;
        [SerializeField] private Toggle _compareListLayerToggle = null;

        [SerializeField] private Graphic[] _candidateFill = null;
        [SerializeField] private Graphic[] _listFill = null;
        [SerializeField] private Graphic[] _compareListFill = null;

        [SerializeField] private LocalizedText _candidateText = null;
        [SerializeField] private LocalizedText _userText = null;
        [SerializeField] private LocalizedText _previousUserText = null;
        [SerializeField] private LocalizedText _listText = null;
        [SerializeField] private LocalizedText _compareListText = null;

        private SmartSpiderController SmartSpider { get; set; }
        private string[] Layers { get; set; }

        public void Initialize(SmartSpiderController controller)
        {
            SmartSpider = controller;

            _listText.key = LKey.Components.ElectionList.ListNumber.Official;
            _userText.key = LKey.Views.Candidate.TogglePlayer;
            _previousUserText.key = LKey.Views.Candidate.TogglePlayerPrevious;

            if (_candidateLayerToggle) _candidateLayerToggle.onValueChanged.AddListener(OnCandidateLayerToggled);
            if (_userLayerToggle) _userLayerToggle.onValueChanged.AddListener(OnUserLayerToggled);
            if (_previousUserLayerToggle)
                _previousUserLayerToggle.onValueChanged.AddListener(OnPreviousUserLayerToggled);
            if (_listLayerToggle) _listLayerToggle.onValueChanged.AddListener(OnListLayerToggled);
            if (_compareListLayerToggle) _compareListLayerToggle.onValueChanged.AddListener(OnCompareListLayerToggled);

            controller.CandidateChanged += OnCandidateChanged;
            controller.PreviousUserSpiderChanged += OnPreviousUserSpiderChanged;
            controller.ListChanged += OnListChanged;
            controller.CompareListChanged += OnCompareListChanged;
        }

        private void OnCandidateChanged()
        {
            var candidate = SmartSpider.Candidate;
            _candidateText.key = new LocalizationKey {fallback = candidate != null ? candidate.firstName : ""};
            foreach(var i in _candidateFill) i.color = candidate != null ? candidate.GetPartyColor() : Color.gray;

            _candidateLayerToggle.gameObject.SetActive(candidate != null);
        }

        private void OnPreviousUserSpiderChanged()
        {
            var smartSpider = SmartSpider.PreviousUserSpider;
            _previousUserLayerToggle.gameObject.SetActive(smartSpider != null);
        }

        private void OnListChanged()
        {
            var list = SmartSpider.List;
            if (list != null) _listText.SetVariable("number", list.number.ToString());
            _listLayerToggle.gameObject.SetActive(list != null);
            var listColor = list != null ? list.GetColor() : Color.gray;
            foreach (var graphic in _listFill) graphic.color = listColor;
        }

        private void OnCompareListChanged()
        {
            var list = SmartSpider.List;
            if (list != null) _compareListText.SetVariable("number", list.number.ToString());
            _compareListLayerToggle.gameObject.SetActive(list != null);
            var listColor = list != null ? list.GetColor() : Color.gray;
            foreach (var graphic in _compareListFill) graphic.color = listColor;
        }

        private void OnCandidateLayerToggled(bool isOn)
        {
            SmartSpider.ShowCandidate(SmartSpider.Candidate, isOn);
        }

        private void OnUserLayerToggled(bool isOn)
        {
            SmartSpider.ShowUser(isOn);
        }

        private void OnPreviousUserLayerToggled(bool isOn)
        {
            SmartSpider.ShowPreviousUser(SmartSpider.PreviousUserSpider, isOn);
        }

        private void OnListLayerToggled(bool isOn)
        {
            SmartSpider.ShowList(SmartSpider.List, isOn);
        }

        private void OnCompareListLayerToggled(bool isOn)
        {
            SmartSpider.ShowCompareList(SmartSpider.CompareList, isOn);
        }
    }
}