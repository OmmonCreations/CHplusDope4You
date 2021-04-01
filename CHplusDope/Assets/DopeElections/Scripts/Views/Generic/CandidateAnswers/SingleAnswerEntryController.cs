using UnityEngine;
using UnityEngine.UI;

namespace DopeElections
{
    public class SingleAnswerEntryController : MonoBehaviour
    {
        [SerializeField] private GameObject _candidateCheckmark = null;
        [SerializeField] private GameObject _userCheckmark = null;
        [SerializeField] private int _value = 0;
        [SerializeField] private Graphic[] _accentGraphics = null;

        private Color _accentColor;
        
        public int Value => _value;
        
        public bool CandidateChecked
        {
            get => _candidateCheckmark.activeSelf;
            set => _candidateCheckmark.SetActive(value);
        }
        
        public bool UserChecked
        {
            get => _userCheckmark.activeSelf;
            set => _userCheckmark.SetActive(value);
        }
        
        public Color AccentColor
        {
            get => _accentColor;
            set => ApplyAccentColor(value);
        }

        private void ApplyAccentColor(Color color)
        {
            _accentColor = color;
            foreach(var g in _accentGraphics) g.color = color;
        }
    }
}