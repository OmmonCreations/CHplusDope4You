using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pagination
{
    public class PageIndexController : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private TMP_Text _normalLabel = null;
        [SerializeField] private TMP_Text _activeLabel = null;
        [SerializeField] private GameObject _normalState = null;
        [SerializeField] private GameObject _activeState = null;

        private string _label;
        private int _index;

        public UnityEvent onClick => _button.onClick;

        public string Label
        {
            get => _label;
            set => ApplyLabel(value);
        }
        
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public void SetActive(bool active)
        {
            _normalState.SetActive(!active);
            _activeState.SetActive(active);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        private void ApplyLabel(string label)
        {
            _label = label;
            _normalLabel.text = label;
            _activeLabel.text = label;
        }
    }
}