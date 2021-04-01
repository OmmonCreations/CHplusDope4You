using System;
using System.Collections.Generic;
using Essentials;
using Essentials.Trigonometry;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensions.Lines.LineBuilders
{
    internal static class AngleCornerLine
    {
        internal static List<Triangle> Build(VertexHelper vh, List<Vector2> vertices, List<Vector2> normals,
            Line.WidthMode mode, Vector2 widthVector, float width, Line.Side side, bool loop, Color color)
        {
            var triangles = new List<Triangle>();
            var vertexUvStep = 1f / vertices.Count;

            for (var i = 0; i < vertices.Count - (loop ? 0 : 1); i++)
            {
                var b = vertices[i];
                var c = i + 1 < vertices.Count ? vertices[i + 1] : vertices[0];

                Vector2 legA;
                Vector2 legB;

                switch (mode)
                {
                    case Line.WidthMode.Tangent:
                    {
                        var line = (c - b);
                        var lineLength = line.magnitude;
                        var delta = line.normalized;
                        var perpendicular = new Vector2(-delta.y, delta.x) / 2 * width;
                        var normalA = normals[i];
                        var normalB = i + 1 < normals.Count ? normals[i + 1] : normals[0];

                        var angleA = 90 - Vector2.SignedAngle(perpendicular, normalA);
                        var angleB = 90 - Vector2.SignedAngle(perpendicular, normalB);

                        var radiansA = angleA / 180 * Mathf.PI;
                        var radiansB = angleB / 180 * Mathf.PI;

                        var legALength = 1 / Mathf.Sin(radiansA) * width / 2;
                        var legBLength = 1 / Mathf.Sin(radiansB) * width / 2;

                        legA = normalA * Mathf.Min(legALength, lineLength);
                        legB = normalB * Mathf.Min(legBLength, lineLength);
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

                var aPos = GetTriangleCornerInside(b, legA, side);
                var bPos = GetTriangleCornerInside(c, legB, side);
                var cPos = GetTriangleCornerOutside(c, legB, side);
                var dPos = GetTriangleCornerOutside(b, legA, side);

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
                var bIndex = i * 4 + 3;
                var cIndex = i * 4 + 2;
                var dIndex = i * 4 + 1;

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