using UnityEngine;

namespace PopupInfos
{
    public struct PopupInfoData
    {
        public string Text { get; set; }
        public Sprite Sprite { get; set; }

        public Color Color
        {
            get => _colorSet ? _color : Color.white;
            set
            {
                _color = value;
                _colorSet = true;
            }
        }

        private Color _color;
        private bool _colorSet;
    }
}