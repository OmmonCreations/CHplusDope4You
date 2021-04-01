using System;
using System.Collections.Generic;
using Essentials;
using Essentials.Trigonometry;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensions.Lines.LineBuilders
{
    public class NoCornerLine
    {
        internal static List<Triangle> Build(VertexHelper vh, List<Vector2> vertices, List<Vector2> normals,
            Line.WidthMode mode, Vector2 widthVector, float width, Line.Side side, bool loop, Color color)
        {
            var triangles = new List<Triangle>();
            var vertexUvStep = 1f / vertices.Count;

            for (var i = 0; i < vertices.Count - (loop ? 0 : 1); i++)
            {
                var a = vertices[i];
                var b = i + 1 < vertices.Count ? vertices[i + 1] : vertices[0];

                Vector2 legA;
                Vector2 legB;

                switch (mode)
                {
                    case Line.WidthMode.Tangent:
                    {
                        var delta = (b - a).normalized;
                        var perpendicular = -new Vector2(-delta.y, delta.x) / 2 * width;

                        legA = perpendicular;
                        legB = perpendicular;
                        break;
                    }
                    case Line.WidthMode.Fixed:
                    {
                        legA = widthVector * (width / 2);
                        legB = legA;
                        break;
                    }
                    default:
                    {
                        throw new NotImplementedException();
                    }
                }

                // Points:
                // ---------  a   ---------  b   ---------  c
                //
                // Vertices:
                // --------- dPos --------- cPos ---------
                // --------- aPos --------- bPos ---------

                var aPos = GetTriangleCornerInside(a, legA, side);
                var bPos = GetTriangleCornerInside(b, legB, side);
                var cPos = GetTriangleCornerOutside(b, legB, side);
                var dPos = GetTriangleCornerOutside(a, legA, side);

                var uiVertexA = UIVertex.simpleVert;
                var uiVertexB = UIVertex.simpleVert;
                var uiVertexC = UIVertex.simpleVert;
                var uiVertexD = UIVertex.simpleVert;
                uiVertexA.color = color;
                uiVertexB.color = color;
                uiVertexC.color = color;
                uiVertexD.color = color;
                uiVertexA.position = aPos;
                uiVertexB.position = bPos;
                uiVertexC.position = cPos;
                uiVertexD.position = dPos;
                uiVertexA.uv0 = new Vector2(i * vertexUvStep, 0);
                uiVertexB.uv0 = new Vector2((i + 1) * vertexUvStep, 0);
                uiVertexC.uv0 = new Vector2((i + 1) * vertexUvStep, 1);
                uiVertexD.uv0 = new Vector2(i * vertexUvStep, 1);
                vh.AddVert(uiVertexA);
                vh.AddVert(uiVertexB);
                vh.AddVert(uiVertexC);
                vh.AddVert(uiVertexD);

                var aIndex = i * 4 + 0;
                var bIndex = i * 4 + 1;
                var cIndex = i * 4 + 2;
                var dIndex = i * 4 + 3;

                vh.AddTriangle(aIndex, bIndex, cIndex);
                vh.AddTriangle(cIndex, dIndex, aIndex);

                triangles.Add(new Triangle(aPos, bPos, cPos));
                triangles.Add(new Triangle(cPos, dPos, aPos));
            }

            return triangles;
        }

        private static Vector2 GetTriangleCornerInside(Vector2 point, Vector2 leg, Line.Side side)
        {
            switch (side)
            {
                case Line.Side.Inside:
                    return point - leg * 2;
                case Line.Side.Outside:
                    return point;
                default:
                    return point - leg;
            }
        }

        private static Vector2 GetTriangleCornerOutside(Vector2 point, Vector2 leg, Line.Side side)
        {
            switch (side)
            {
                case Line.Side.Inside:
                    return point;
                case Line.Side.Outside:
                    return point + leg * 2;
                default:
                    return point + leg;
            }
        }
    }
}