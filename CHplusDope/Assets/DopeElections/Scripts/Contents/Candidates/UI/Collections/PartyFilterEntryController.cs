using DopeElections.Answer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.Candidates
{
    public class PartyFilterEntryController : MonoBehaviour
    {
        public delegate void ToggleEvent(bool selected);

        public event ToggleEvent Changed = delegate { };

        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private TMP_Text _label = null;
        [SerializeField] private GameObject _activeState = null;

        public Party Party { get; private set; }
        
        public bool Selected
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        }

        public void Initialize(Party party)
        {
            Party = party;
            _label.text = party.abbr;
            _toggle.onValueChanged.AddListener(OnValueChanged);
            _toggle.SetIsOnWithoutNotify(false);
            UpdateState();
        }

        private void OnValueChanged(bool value)
        {
            UpdateState();
            Changed(value);
        }

        private void UpdateState()
        {
            var selected = Selected;
            if(_activeState) _activeState.SetActive(selected);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
        
        public void SetValueWithoutNotify(bool value)
        {
            _toggle.SetIsOnWithoutNotify(value);
            UpdateState();
        }
    }
}