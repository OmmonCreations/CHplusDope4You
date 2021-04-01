using UnityEngine;

namespace UIExtensions.Lines
{
    [System.Serializable]
    public class LineHandle
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Vector2 _position;
        [SerializeField] private Vector2 _direction;
        [SerializeField] private Vector2 _offset;

        public bool IsWorldSpace
        {
            get { return _rectTransform; }
        }
        
        public Vector2 Position
        {
            get
            {
                if (!IsWorldSpace) return _position;
                var position = _position;
                var rect = _rectTransform.rect;
                position -= _rectTransform.pivot;
                position.x *= rect.width;
                position.y *= rect.height;
                return (Vector2) _rectTransform.TransformPoint(position) + _offset;
            }
        }
        
        public Vector2 Direction
        {
            get { return _direction; }
        }

        public LineHandle(Vector2 position, Vector2 direction) : this(null, position, direction, Vector2.zero)
        {
        }

        public LineHandle(Vector2 position, Vector2 direction, Vector2 offset) : this(null, position, direction, offset)
        {
        }

        public LineHandle(RectTransform rectTransform, Vector2 position, Vector2 direction) : this(rectTransform, position, direction, Vector2.zero)
        {
            
        }

        public LineHandle(RectTransform rectTransform, Vector2 position, Vector2 direction, Vector2 offset)
        {
            _rectTransform = rectTransform;
            _position = position;
            _direction = direction;
            _offset = offset;
        }
    }
}