using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DopeElections.ElectionLists.UI
{
    public class ActionButton : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private Image _colorTarget = null;
        [SerializeField] private Image _iconTarget = null;

        public UnityEvent onClick => _button.onClick;
        
        public Sprite Icon
        {
            get => _iconTarget.sprite;
            set => _iconTarget.sprite = value;
        }

        public Color Color
        {
            get => _colorTarget.color;
            set => _colorTarget.color = value;
        }
    }
}