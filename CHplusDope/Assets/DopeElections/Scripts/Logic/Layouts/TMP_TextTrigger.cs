using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(TMP_Text))]
    [ExecuteAlways]
    public class TMP_TextTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent<string> _onValueChanged = new UnityEvent<string>();

        private TMP_Text _text;
        private string _value;

        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
            if (!_text)
            {
                enabled = false;
                return;
            }

            _value = _text.text;
            TriggerUpdate();
        }

        private void LateUpdate()
        {
            if (!_text)
            {
                enabled = false;
                return;
            }

            var value = _text.text;
            if (_value == value) return;
            _value = value;
            TriggerUpdate();
        }

        private void OnDisable()
        {
            _text = null;
            _value = null;
        }

        public void TriggerUpdate()
        {
            _onValueChanged.Invoke(_value);
        }
    }
}