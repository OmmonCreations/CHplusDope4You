using Localizator;
using SortableCollections;
using TMPro;
using UnityEngine;

namespace DopeElections.Candidates
{
    public class NameFilterController : FilterController<NameFilter>
    {
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private LocalizedText _placeholderText = null;

        protected override void OnAwake()
        {
            base.OnAwake();
            _inputField.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if(_inputField==null) Debug.LogError("InputField is null!");
            _inputField.SetTextWithoutNotify(Filter.State.Key);
            _placeholderText.key = Filter.Label;
        }

        private void OnValueChanged(string value)
        {
            Filter.State = new NameFilter.FilterState(value);
        }
    }
}