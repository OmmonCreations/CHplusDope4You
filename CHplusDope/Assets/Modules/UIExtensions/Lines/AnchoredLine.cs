using System.Linq;
using UnityEngine;

namespace UIExtensions.Lines
{
    public class AnchoredLine : Line
    {
        [SerializeField] private LineHandle[] _handles = null;

        private bool _handlesDirty = false;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            RecalculateRect();
        }

        private void LateUpdate()
        {
            if (_handlesDirty)
            {
                _handlesDirty = false;
                RecalculateLine();
            }
            if (transform.hasChanged)RecalculateRect();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            RecalculateRect();
        }

        public void SetHandle(int index, LineHandle handle)
        {
            if (index < 0 || index >= _handles.Length) return;
            _handles[index] = handle;
            _handlesDirty = true;
        }

        protected virtual void RecalculateLine()
        {
            if (_handles == null || _handles.Length < 2) return;

            var rectTransform = this.rectTransform;

            var points = new Vector2[_handles.Length];
            var tangents = new Vector2[_handles.Length];
            var pivot = rectTransform.pivot;
            var rect = rectTransform.rect;

            for (var i = 0; i < _handles.Length; i++)
            {
                var handle = _handles[i];
                var position = handle.Position;
                if (handle.IsWorldSpace)
                {
                    var transformed = rectTransform.InverseTransformPoint(position);
                    transformed.x /= rect.width;
                    transformed.y /= rect.height;
                    transformed += (Vector3) pivot;
                    position = transformed;
                }

                points[i] = TransformVector(position, pivot, rect.width, rect.height);
                tangents[i] = handle.Direction.normalized;
            }

            Points = points;
            Tangents = tangents;
        }

        private static Vector2 TransformVector(Vector2 relative, Vector2 pivot, float width, float height)
        {
            var result = relative;
            result -= pivot;
            result.x *= width;
            result.y *= height;
            return result;
        }

        public virtual void RecalculateRect()
        {
            var rectTransform = this.rectTransform;
            var parent = rectTransform.parent as RectTransform;
            if (parent==null || _handles==null || _handles.Length==0) return;

            var parentPivot = parent.pivot;
            var parentRect = parent.rect;
            
            var points = new Vector2[_handles.Length];
            
            for (var i = 0; i < _handles.Length; i++)
            {
                var handle = _handles[i];
                var position = handle.Position;
                if (handle.IsWorldSpace)
                {
                    var transformed = parent.InverseTransformPoint(position);
                    if(parentRect.width>0) transformed.x /= parentRect.width;
                    if(parentRect.height>0) transformed.y /= parentRect.height;
                    transformed += (Vector3) parentPivot;
                    position = transformed;
                }

                points[i] = position;
            }

            var min = points.FirstOrDefault();
            var max = min;
            foreach (var point in points)
            {
                min.x = Mathf.Min(min.x, point.x);
                min.y = Mathf.Min(min.y, point.y);
                max.x = Mathf.Max(max.x, point.x);
                max.y = Mathf.Max(max.y, point.y);
            }

            if (float.IsNaN(min.x)) min.x = 0;
            if (float.IsNaN(min.y)) min.y = 0;
            if (float.IsNaN(max.x)) max.x = 1;
            if (float.IsNaN(max.y)) max.y = 1;

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.offsetMin = new Vector2(-20,-20);
            rectTransform.offsetMax = new Vector2(20,20);
            SetVerticesDirty();
        }
    }
}