using TMPro;
using UnityEngine;

namespace DopeElections.Layouts
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextSynchronizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _target = null;

        private TMP_Text _text;
        
        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
            UpdateState();
        }

        public void TriggerUpdate()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (_text && _target) _target.text = _text.text;
        }
    }
}