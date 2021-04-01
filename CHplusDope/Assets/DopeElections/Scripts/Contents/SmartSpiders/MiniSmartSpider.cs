using System.Collections.Generic;
using System.Linq;
using Essentials;
using Essentials.Trigonometry;
using UIExtensions.Lines;
using UnityEngine;
using UnityEngine.UI;

namespace DopeElections.SmartSpiders
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class MiniSmartSpider : MaskableGraphic
    {
        [SerializeField] private float[] _value = new float[8];
        [SerializeField] private Line _outline = null;
        [SerializeField] private Line _dropshadow = null;

        private Vector2[] _points;
        private Triangle[] _triangles;

        public float[] Value
        {
            get => _value;
            set => ApplyValue(value);
        }

        private float radius
        {
            get
            {
                var rectTransform = this.rectTransform;
                var rect = rectTransform.rect;
                return Mathf.Min(rect.width, rect.height);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetVerticesDirty();
            if (_outline) _outline.enabled = true;
            if (_dropshadow) _dropshadow.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_outline) _outline.enabled = false;
            if (_dropshadow) _dropshadow.enabled = false;
        }

        private void ApplyValue(float[] value)
        {
            _value = value != null ? value : new float[8];
            UpdatePoints();
            if (_outline)
            {
                _outline.Points = _points;
            }
            if (_dropshadow)
            {
                _dropshadow.Points = _points;
            }

            SetVerticesDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdatePoints();
            if (_outline)
            {
                _outline.Points = _points;
                _outline.RecalculateLineTangents();
            }
            if (_dropshadow)
            {
                _dropshadow.Points = _points;
                _dropshadow.RecalculateLineTangents();
            }

            SetVerticesDirty();
        }
#endif

        private void UpdatePoints()
        {
            if (_points == null || _points.Length != Value.Length) _points = new Vector2[Value.Length];
            var values = Value;
            var points = _points;
            var step = 1f / values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var radians = Mathf.PI / 2 + -(i * step * 2 * Mathf.PI);
                var x = Mathf.Cos(radians) / 2 * value;
                var y = Mathf.Sin(radians) / 2 * value;
                points[i] = new Vector2(x + 0.5f, y + 0.5f);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var points = Value;
            if (points.Length == 0)
            {
                // Debug.LogWarning("No points to render.");
                return;
            }

            var rectTransform = this.rectTransform;
            var rect = rectTransform.rect;
            var color = this.color;
            var pivot = rectTransform.pivot;
            var radius = this.radius;
            var centerVector = new Vector2(
                Mathf.Max(rect.width - rect.height, 0) / 2,
                Mathf.Max(rect.height - rect.width, 0) / 2
            );
            var offset = -new Vector2(pivot.x * rect.width, pivot.y * rect.height) + centerVector;

            var vertices = new List<Vector2> {new Vector2(0.5f, 0.5f) * radius + offset};
            if (_points == null) UpdatePoints();
            vertices.AddRange(_points.Select(p => (p * radius) + offset));

            foreach (var v in vertices)
                vh.AddVert(new UIVertex()
                {
                    position = v,
                    color = color,
                    uv0 = v,
                    uv1 = v,
                    uv2 = v,
                    uv3 = v
                });

            var triangles = new List<Triangle>();
            for (var i = 0; i < points.Length; i++)
            {
                var current = i + 1;
                var next = i + 2;
                if (next >= vertices.Count) next = 1;
                triangles.Add(new Triangle(vertices[0], vertices[next],
                    vertices[current])); // order is counter clockwise
                vh.AddTriangle(0, next, current);
            }

            // keep the triangles around for raycasting against the mesh, e.g. to check mouse hover
            _triangles = triangles.ToArray();

            // Debug.Log(string.Join("\n", _triangles.Select(t => t.a + "," + t.b + "," + t.c)));
        }
    }
}