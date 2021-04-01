using System.Collections.Generic;
using Essentials;
using Essentials.Trigonometry;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensions.Lines.LineBuilders
{
    internal static class RoundCornerLine
    {
        internal static List<Triangle> BuildCenter(VertexHelper vh, List<Vector2> vertices, List<Vector2> normals,
            float width, bool loop, Color color)
        {
            var triangles = new List<Triangle>();
            var vertexUvStep = 1f / vertices.Count;

            var vertexIndexer = 0;
            for (var i = 0; i < vertices.Count - (loop ? 0 : 1); i++)
            {
                var a = vertices[i];
                var b = i + 1 < vertices.Count ? vertices[i + 1] : vertices[0];
                var line = b - a;
                var lineLength = line.magnitude;
                var delta = line.normalized;
                // if (delta.magnitude < width / 2) continue;

                var c = vertices[MathUtil.Wrap(i + 2, vertices.Count)]; // needed to calculate corner cap on b

                var perpendicular = new Vector2(-delta.y, delta.x).normalized * width / 2;
                var normalA = normals[i];
                var normalB = i + 1 < normals.Count ? normals[i + 1] : normals[0];

                var angleA = 90 - Vector2.SignedAngle(perpendicular, normalA);
                var angleB = 90 - Vector2.SignedAngle(perpendicular, normalB);

                // Debug.Log(angleA+", "+angleB);

                var legALength = (1 / Mathf.Sin(angleA / 180 * Mathf.PI)) * width;
                var legBLength = (1 / Mathf.Sin(angleB / 180 * Mathf.PI)) * width;

                var innerLegA = normalA * Mathf.Min(legALength, lineLength) / 2;
                var innerLegB = normalB * Mathf.Min(legBLength, lineLength) / 2;
                var outerLegA = perpendicular;
                var outerLegB = perpendicular;

                // Points:
                // ---------  a   ---------  b   ---------  c
                //
                // Vertices:
                // --------- cPos --------- fPos ---------
                // - - - - - bPos - - - - - ePos - - - - -
                // --------- aPos --------- dPos ---------

                var aPos = GetTriangleCornerInside(a, (angleA > 90 ? outerLegA : innerLegA), innerLegA,
                    Line.Side.Center);
                var bPos = GetTriangleCornerCenter(a, innerLegA, Line.Side.Center);
                var cPos = GetTriangleCornerOutside(a, (angleA < 90 ? outerLegA : innerLegA), innerLegA,
                    Line.Side.Center);
                var dPos = GetTriangleCornerInside(b, (angleB < 90 ? outerLegB : innerLegB), innerLegB,
                    Line.Side.Center);
                var ePos = GetTriangleCornerCenter(b, innerLegB, Line.Side.Center);
                var fPos = GetTriangleCornerOutside(b, (angleB > 90 ? outerLegB : innerLegB), innerLegB,
                    Line.Side.Center);

                var uiVertexA = UIVertex.simpleVert;
                var uiVertexB = UIVertex.simpleVert;
                var uiVertexC = UIVertex.simpleVert;
                var uiVertexD = UIVertex.simpleVert;
                var uiVertexE = UIVertex.simpleVert;
                var uiVertexF = UIVertex.simpleVert;
                uiVertexA.color = color;
                uiVertexB.color = color;
                uiVertexC.color = color;
                uiVertexD.color = color;
                uiVertexE.color = color;
                uiVertexF.color = color;
                uiVertexA.position = aPos;
                uiVertexB.position = bPos;
                uiVertexC.position = cPos;
                uiVertexD.position = dPos;
                uiVertexE.position = ePos;
                uiVertexF.position = fPos;
                uiVertexA.uv0 = new Vector2(i * vertexUvStep, 0);
                uiVertexB.uv0 = new Vector2(i * vertexUvStep, 0.5f);
                uiVertexC.uv0 = new Vector2(i * vertexUvStep, 1);
                uiVertexD.uv0 = new Vector2((i + 1) * vertexUvStep, 0);
                uiVertexE.uv0 = new Vector2((i + 1) * vertexUvStep, 0.5f);
                uiVertexF.uv0 = new Vector2((i + 1) * vertexUvStep, 1);
                vh.AddVert(uiVertexA);
                vh.AddVert(uiVertexB);
                vh.AddVert(uiVertexC);
                vh.AddVert(uiVertexD);
                vh.AddVert(uiVertexE);
                vh.AddVert(uiVertexF);

                var aIndex = vertexIndexer + 0;
                var bIndex = vertexIndexer + 1;
                var cIndex = vertexIndexer + 2;
                var dIndex = vertexIndexer + 3;
                var eIndex = vertexIndexer + 4;
                var fIndex = vertexIndexer + 5;

                vh.AddTriangle(aIndex, bIndex, eIndex);
                vh.AddTriangle(aIndex, eIndex, dIndex);
                vh.AddTriangle(bIndex, cIndex, fIndex);
                vh.AddTriangle(bIndex, fIndex, eIndex);

                triangles.Add(new Triangle(aPos, bPos, ePos));
                triangles.Add(new Triangle(aPos, ePos, dPos));
                triangles.Add(new Triangle(bPos, cPos, fPos));
                triangles.Add(new Triangle(bPos, fPos, ePos));

                vertexIndexer += 6;

                // if this was the last part skip corner generation
                if (!loop && i + 2 >= vertices.Count)
                {
                    continue;
                }

                // build triangle fan
                var nextDelta = c - b;
                var fanAngle = -Vector2.SignedAngle(nextDelta, delta);
                var steps = Mathf.CeilToInt(Mathf.Abs(fanAngle / 15));
                var step = fanAngle / steps;
                var centerVertex = uiVertexE;
                var centerIndex = eIndex;
                var clockwise = angleB > 90;
                var lastVertex = clockwise ? uiVertexF : uiVertexD;
                var lastIndex = clockwise ? fIndex : dIndex;
                //var centerX = centerVertex.position.x;
                //var centerY = centerVertex.position.y;
                for (var j = 0; j < steps; j++)
                {
                    var lastDelta = (Vector2) (lastVertex.position - centerVertex.position);
                    var nextPosition = centerVertex.position + (Vector3) lastDelta.Rotate(step);
                    var nextIndex = vertexIndexer;
                    var nextVertex = UIVertex.simpleVert;
                    nextVertex.position = nextPosition;
                    nextVertex.uv0 = angleB > 90 ? Vector2.up : Vector2.zero;
                    nextVertex.color = color;
                    vertexIndexer++;
                    vh.AddVert(nextVertex);
                    if (clockwise)
                    {
                        vh.AddTriangle(centerIndex, lastIndex, nextIndex);
                        triangles.Add(new Triangle(centerVertex.position, lastVertex.position, nextVertex.position));
                    }
                    else
                    {
                        vh.AddTriangle(centerIndex, nextIndex, lastIndex);
                        triangles.Add(new Triangle(centerVertex.position, nextVertex.position, lastVertex.position));
                    }

                    lastVertex = nextVertex;
                    lastIndex = nextIndex;
                }
            }

            return triangles;
        }

        internal static List<Triangle> BuildSide(VertexHelper vh, List<Vector2> vertices, List<Vector2> normals,
            float width, Line.Side side, bool loop, Color color)
        {
            var triangles = new List<Triangle>();
            var vertexUvStep = 1f / vertices.Count;

            var vertexIndexer = 0;
            for (var i = 0; i < vertices.Count - (loop ? 0 : 1); i++)
            {
                var a = vertices[i];
                var b = i + 1 < vertices.Count ? vertices[i + 1] : vertices[0];
                var line = b - a;
                var lineLength = line.magnitude;
                var delta = line.normalized;
                // if (delta.magnitude < width / 2) continue;

                var c = vertices[MathUtil.Wrap(i + 2, vertices.Count)]; // needed to calculate corner cap on b

                var perpendicular = new Vector2(-delta.y, delta.x).normalized * width / 2;
                var normalA = normals[i];
                var normalB = i + 1 < normals.Count ? normals[i + 1] : normals[0];

                var angleA = 90 - Vector2.SignedAngle(perpendicular, normalA);
                var angleB = 90 - Vector2.SignedAngle(perpendicular, normalB);

                // Debug.Log(angleA+", "+angleB);

                var legALength = (1 / Mathf.Sin(angleA / 180 * Mathf.PI)) * width;
                var legBLength = (1 / Mathf.Sin(angleB / 180 * Mathf.PI)) * width;

                var innerLegA = normalA * Mathf.Min(legALength, lineLength) / 2;
                var innerLegB = normalB * Mathf.Min(legBLength, lineLength) / 2;
                var outerLegA = perpendicular;
                var outerLegB = perpendicular;

                // Points:
                // ---------  a   ---------  b   ---------  c
                //
                // Vertices:
                // --------- cPos --------- fPos ---------
                // --------- aPos --------- dPos ---------

                var aPos = GetTriangleCornerInside(a, (angleA > 90 ? outerLegA : innerLegA), innerLegA, side);
                var cPos = GetTriangleCornerOutside(a, (angleA < 90 ? outerLegA : innerLegA), innerLegA, side);
                var dPos = GetTriangleCornerInside(b, (angleB < 90 ? outerLegB : innerLegB), innerLegB, side);
                var fPos = GetTriangleCornerOutside(b, (angleB > 90 ? outerLegB : innerLegB), innerLegB, side);

                var uiVertexA = UIVertex.simpleVert;
                var uiVertexC = UIVertex.simpleVert;
                var uiVertexD = UIVertex.simpleVert;
                var uiVertexF = UIVertex.simpleVert;
                uiVertexA.color = color;
                uiVertexC.color = color;
                uiVertexD.color = color;
                uiVertexF.color = color;
                uiVertexA.position = aPos;
                uiVertexC.position = cPos;
                uiVertexD.position = dPos;
                uiVertexF.position = fPos;
                uiVertexA.uv0 = new Vector2(i * vertexUvStep, 0);
                uiVertexC.uv0 = new Vector2(i * vertexUvStep, 1);
                uiVertexD.uv0 = new Vector2((i + 1) * vertexUvStep, 0);
                uiVertexF.uv0 = new Vector2((i + 1) * vertexUvStep, 1);
                vh.AddVert(uiVertexA);
                vh.AddVert(uiVertexC);
                vh.AddVert(uiVertexD);
                vh.AddVert(uiVertexF);

                var aIndex = vertexIndexer + 0;
                var cIndex = vertexIndexer + 1;
                var dIndex = vertexIndexer + 2;
                var fIndex = vertexIndexer + 3;

                vh.AddTriangle(aIndex, cIndex, fIndex);
                vh.AddTriangle(aIndex, fIndex, dIndex);

                triangles.Add(new Triangle(aPos, cPos, fPos));
                triangles.Add(new Triangle(aPos, fPos, dPos));

                vertexIndexer += 4;

                // if this was the last part skip corner generation
                if (!loop && i + 2 >= vertices.Count || (side == Line.Side.Outside && angleB <= 90))
                {
                    continue;
                }

                // build triangle fan
                var nextDelta = c - b;
                var fanAngle = -Vector2.SignedAngle(nextDelta, delta);
                var steps = Mathf.CeilToInt(Mathf.Abs(fanAngle / 15));
                var step = fanAngle / steps;
                var centerVertex = angleB > 90 ? uiVertexD : uiVertexF;
                var centerIndex = angleB > 90 ? dIndex : fIndex;
                var clockwise = angleB > 90;
                var lastVertex = clockwise ? uiVertexF : uiVertexD;
                var lastIndex = clockwise ? fIndex : dIndex;

                for (var j = 0; j < steps; j++)
                {
                    var lastDelta = (Vector2) (lastVertex.position - centerVertex.position);
                    var nextPosition = centerVertex.position + (Vector3) lastDelta.Rotate(step);
                    var nextIndex = vertexIndexer;
                    var nextVertex = UIVertex.simpleVert;
                    nextVertex.position = nextPosition;
                    nextVertex.uv0 = angleB > 90 ? Vector2.up : Vector2.zero;
                    nextVertex.color = color;
                    vertexIndexer++;
                    vh.AddVert(nextVertex);
                    if (clockwise)
                    {
                        vh.AddTriangle(centerIndex, lastIndex, nextIndex);
                        triangles.Add(new Triangle(centerVertex.position, lastVertex.position, nextVertex.position));
                    }
                    else
                    {
                        vh.AddTriangle(centerIndex, nextIndex, lastIndex);
                        triangles.Add(new Triangle(centerVertex.position, nextVertex.position, lastVertex.position));
                    }

                    lastVertex = nextVertex;
                    lastIndex = nextIndex;
                }
            }

            return triangles;
        }

        private static Vector2 GetTriangleCornerInside(Vector2 point, Vector2 leg, Vector2 innerLeg, Line.Side side)
        {
            switch (side)
            {
                case Line.Side.Inside:
                    return point - leg * 2;
                case Line.Side.Outside:
                    //return point - leg * 2 + innerLeg * 2;
                    return point;
                default:
                    return point - leg;
            }
        }

        private static Vector2 GetTriangleCornerCenter(Vector2 point, Vector2 innerLeg, Line.Side side)
        {
            switch (side)
            {
                case Line.Side.Inside:
                    return point - innerLeg;
                case Line.Side.Outside:
                    return point + innerLeg;
                default:
                    return point;
            }
        }

        private static Vector2 GetTriangleCornerOutside(Vector2 point, Vector2 outerLeg, Vector2 innerLeg,
            Line.Side side)
        {
            switch (side)
            {
                case Line.Side.Inside:
                    return point - innerLeg * 2 + outerLeg * 2;
                case Line.Side.Outside:
                    return point + outerLeg * 2;
                default:
                    return point + outerLeg;
            }
        }
    }
}