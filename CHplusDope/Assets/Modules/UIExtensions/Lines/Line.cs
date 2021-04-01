using System;
using System.Collections.Generic;
using System.Linq;
using Essentials;
using Essentials.Trigonometry;
using UIExtensions.Lines.LineBuilders;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensions.Lines
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class Line : MaskableGraphic
    {
        [SerializeField] private float _width = 1;
        [SerializeField] private int _resolution = 16;
        [SerializeField] private int _roundness = 100;

        [SerializeField] private WidthMode _mode = WidthMode.Tangent;
        [SerializeField] private LineType _type = LineType.Straight;
        [SerializeField] private CoordinateSystem _coordinates = CoordinateSystem.Pixel;
        [SerializeField] private CornerType _corners = CornerType.Angle;
        [SerializeField] private Side _side = Side.Center;
        [SerializeField] private Vector2 _widthVector = Vector2.up;
        [SerializeField] private bool _loop = false;

        [Tooltip("Line Anchors")] [SerializeField]
        private Vector2[] _points = {Vector2.zero, Vector2.right};

        [Tooltip("Tangents for Bezier calculation")] [SerializeField]
        private Vector2[] _tangents = {Vector2.right, Vector2.right};

        private Triangle[] _triangles;

        public int Roundness
        {
            get => _roundness;
            set => _roundness = value;
        }

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                SetVerticesDirty();
            }
        }

        public int Resolution
        {
            get { return _resolution; }
            set
            {
                _resolution = value;
                SetVerticesDirty();
            }
        }

        public WidthMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                SetVerticesDirty();
            }
        }

        public LineType Type
        {
            get => _type;
            set
            {
                _type = value;
                SetVerticesDirty();
            }
        }

        public CoordinateSystem Coordinates
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                SetVerticesDirty();
            }
        }

        public CornerType Corners
        {
            get => _corners;
            set
            {
                _corners = value;
                SetVerticesDirty();
            }
        }

        public Vector3 WidthVector
        {
            get => _widthVector;
            set
            {
                _widthVector = value;
                SetVerticesDirty();
            }
        }

        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            var triangles = _triangles;

            if (triangles == null) return false;

            var result = Raycast(rectTransform, triangles, sp, eventCamera);

            return result;
        }

        private static bool Raycast(RectTransform rectTransform, Triangle[] triangles, Vector2 cursor,
            Camera eventCamera)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                cursor,
                eventCamera,
                out var rectLocalPosition)) return false;
            //var rect = rectTransform.rect;
            // point in (0.0,0.0) to (1.0,1.0) space
            //var relativePosition = new Vector2(rectLocalPosition.x / rect.width, rectLocalPosition.y / rect.height)+ new Vector2(0.5f, 0.5f);
            foreach (var t in triangles)
            {
                if (MathUtil.PointInTriangle(rectLocalPosition, t.a, t.b, t.c)) return true;
            }

            var vertices = triangles.SelectMany(t => new[] {t.a, t.b, t.c}).ToArray();
            //Debug.Log("Cursor: "+ rectLocalPosition);
            //Debug.Log(vertices.Min(v=>v.x)+", "+vertices.Min(v=>v.y)+" - "+vertices.Max(v=>v.x)+", "+vertices.Max(v=>v.y));
            return false;
        }

        public Vector2[] Points
        {
            get { return _points; }
            set
            {
                _points = value;
                SetVerticesDirty();
            }
        }

        public Vector2[] Tangents
        {
            get { return _tangents; }
            set
            {
                _tangents = value;
                SetVerticesDirty();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetVerticesDirty();
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var points = Points.ToList();
            if (points.Count == 0) return;

            var pointTangents = _tangents.ToList();
            var vertices = new List<Vector2>(); // line anchors
            var normals = new List<Vector2>(); // anchor normals

            if (Coordinates == CoordinateSystem.Relative)
            {
                ConvertToPixelCoordinates(points);
                ConvertToPixelTangents(pointTangents);
            }

            // first calculate line vertices
            switch (Type)
            {
                case LineType.Straight:
                {
                    // points can be used directly in this case
                    vertices.AddRange(points);
                    break;
                }
                case LineType.Bezier:
                {
                    // calculate vertices based on points and tangents
                    for (var i = 0; i < points.Count - 1; i++)
                    {
                        var from = points[i];
                        var to = points[i + 1];
                        var roundness = Roundness;
                        var a = from;
                        var b = from + pointTangents[i] * roundness;
                        var c = to - pointTangents[i + 1] * roundness;
                        var d = to;
                        var bezier = new Bezier(a, b, c, d);
                        var length = (b - a).magnitude + (c - b).magnitude + (d - c).magnitude;
                        var resolution = Mathf.RoundToInt(length / 100 * Resolution);
                        var step = 1f / resolution;
                        for (var j = 0; j <= resolution; j++)
                        {
                            var t = j * step;
                            vertices.Add(bezier.Evaluate(t));
                        }
                    }

                    break;
                }
                default: throw new NotImplementedException();
            }

            normals.AddRange(GetNormals(vertices, _loop));

            // cannot create line mesh with less than two vertices
            if (vertices.Count < 2) return;

            List<Triangle> triangles;

            var corners = Type == LineType.Straight ? Corners : CornerType.Angle;
            switch (corners)
            {
                case CornerType.Angle:
                {
                    triangles = AngleCornerLine.Build(vh, vertices, normals, Mode, WidthVector, Width, _side, _loop,
                        color);
                    break;
                }
                case CornerType.Round:
                {
                    triangles = _side==Side.Center 
                        ? RoundCornerLine.BuildCenter(vh, vertices, normals, Width, _loop, color)
                        : RoundCornerLine.BuildSide(vh, vertices, normals, Width, _side, _loop, color);
                    break;
                }
                case CornerType.None:
                {
                    triangles = NoCornerLine.Build(vh, vertices, normals, Mode, WidthVector, Width, _side, _loop,
                        color);
                    break;
                }
                default: throw new NotImplementedException();
            }

            // keep the triangles around for raycasting against the mesh, e.g. to check mouse hover
            _triangles = triangles.ToArray();
        }

        private Vector2[] GetNormals(List<Vector2> vertices, bool loop)
        {
            if (vertices.Count == 0) return new Vector2[0];
            var result = new Vector2[vertices.Count];
            if (!loop && vertices.Count > 1)
            {
                var startTangent = (vertices[1] - vertices[0]).normalized;
                var endTangent = (vertices[vertices.Count - 1] - vertices[vertices.Count - 2]).normalized;
                result[0] = new Vector2(-startTangent.y, startTangent.x);
                result[result.Length - 1] = new Vector2(-endTangent.y, endTangent.x);
            }

            for (var i = 0; i < vertices.Count; i++)
            {
                if (!loop && (i == 0 || i == vertices.Count)) continue;

                var vertex = vertices[i];
                var before = vertices[MathUtil.Wrap(i - 1, vertices.Count)];
                var next = vertices[MathUtil.Wrap(i + 1, vertices.Count)];
                var incoming = (vertex - before).normalized;
                var outgoing = (next - vertex).normalized;
                var tangent = (incoming + outgoing) / 2;
                result[i] = new Vector2(-tangent.y, tangent.x).normalized;
            }

            return result;
        }

        /// <summary>
        /// Calculates absolute coordinates based on an array of relative points within the RectTransform where
        /// Vector(0.0,0.1) in a RectTransform with coordinates {offsetMin:-100,-100;offsetMax:100,100;pivot:0.5,0.5}
        /// becomes Vector(-100,100)
        /// </summary>
        /// <param name="relativePoints"></param>
        private void ConvertToPixelCoordinates(List<Vector2> relativePoints)
        {
            var rectTransform = this.rectTransform;
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;
            var pivot = rectTransform.pivot;
            for (var i = 0; i < relativePoints.Count; i++)
            {
                var p = relativePoints[i];
                relativePoints[i] = new Vector2((p.x - pivot.x) * width, (p.y - pivot.y) * height);
            }
        }

        private void ConvertToPixelTangents(List<Vector2> relativeTangents)
        {
            var rect = rectTransform.rect;
            var aspectRatio = rect.width / rect.height;
            for (var i = 0; i < relativeTangents.Count; i++)
            {
                var t = relativeTangents[i];
                relativeTangents[i] = new Vector2(t.x * aspectRatio, t.y).normalized;
            }
        }

        /// <summary>
        /// Automatically creates line tangents based on points
        /// </summary>
        public void RecalculateLineTangents()
        {
            var points = Points;
            var tangents = new Vector2[points.Length];
            if (points.Length == 0)
            {
                _tangents = tangents;
                return;
            }

            // calculate first tangent
            if (!_loop) tangents[0] = points.Length > 1 ? (points[1] - points[0]).normalized : Vector2.right;

            // calculate tangents
            for (var i = 0; i < points.Length; i++)
            {
                if ((i == 0 || i == points.Length - 1) && !_loop) continue;
                var a = i - 1 >= 0 ? points[i - 1] : points[points.Length - 1]; // previous point
                var b = points[i]; // current point
                var c = i + 1 < points.Length ? points[i + 1] : points[0]; // next point
                var ab = (b - a).normalized;
                var bc = (c - b).normalized;
                var tangent = ((bc + ab) / 2).normalized;
                tangents[i] = tangent;
            }

            // calculate last tangent
            if (!_loop && tangents.Length > 1)
            {
                tangents[points.Length - 1] = (points[points.Length - 1] - points[points.Length - 2]).normalized;
            }

            _tangents = tangents;
            SetVerticesDirty();
        }

        public enum WidthMode
        {
            Tangent,
            Fixed
        }

        public enum LineType
        {
            Straight,
            Bezier
        }

        public enum CoordinateSystem
        {
            Pixel,
            Relative
        }

        public enum CornerType
        {
            /// <summary>
            /// Connects outer corners at the intersection of the respective edges
            /// </summary>
            Angle,

            /// <summary>
            /// Fills outer gap with a triangle fan
            /// </summary>
            Round,

            /// <summary>
            /// Does not fill gaps between segments
            /// </summary>
            None,
        }

        public enum Side
        {
            Center,
            Inside,
            Outside
        }
    }
}