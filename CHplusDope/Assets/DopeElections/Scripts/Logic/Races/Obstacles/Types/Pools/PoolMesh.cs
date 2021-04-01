using System.Collections.Generic;
using UnityEngine;

namespace DopeElections.Races
{
    public static class PoolMesh
    {
        public static void Generate(Mesh mesh, Vector2Int size, float tileSize)
        {
            var vertices = new List<Vector3>();
            var uv = new List<Vector2>();
            var indices = new List<int>();
            var quadCount = 0;
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var position = new Vector2Int(x, y);
                    vertices.AddRange(GetVertices(position, tileSize));
                    uv.AddRange(GetTextureUv(GetTextureTile(position, size)));
                    indices.AddRange(GetQuadIndices(quadCount * 4));
                    quadCount++;
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
        }

        private static IEnumerable<Vector3> GetVertices(Vector2Int position, float tileSize)
        {
            yield return new Vector3(position.x, 0, position.y + 1) * tileSize;
            yield return new Vector3(position.x + 1, 0, position.y + 1) * tileSize;
            yield return new Vector3(position.x + 1, 0, position.y) * tileSize;
            yield return new Vector3(position.x, 0, position.y) * tileSize;
        }

        private static IEnumerable<Vector2> GetTextureUv(Vector2Int position)
        {
            yield return new Vector2(position.x, position.y + 1) * 0.25f;
            yield return new Vector2(position.x + 1, position.y + 1) * 0.25f;
            yield return new Vector2(position.x + 1, position.y) * 0.25f;
            yield return new Vector2(position.x, position.y) * 0.25f;
        }

        private static IEnumerable<int> GetQuadIndices(int start)
        {
            yield return start + 0;
            yield return start + 1;
            yield return start + 2;
            yield return start + 2;
            yield return start + 3;
            yield return start + 0;
        }

        private static Vector2Int GetTextureTile(Vector2Int position, Vector2Int size)
        {
            switch (size.y)
            {
                case 1 when size.x == 1:
                    return new Vector2Int(3, 3);
                case 1 when position.x == 0:
                    return new Vector2Int(0, 3);
                case 1 when position.x < size.x - 1:
                    return new Vector2Int(1, 3);
                case 1:
                    return new Vector2Int(2, 3);
            }

            switch (size.x)
            {
                case 1 when position.y == 0:
                    return new Vector2Int(3, 0);
                case 1 when position.y < size.y - 1:
                    return new Vector2Int(3, 1);
                case 1:
                    return new Vector2Int(3, 2);
            }

            var x = position.x == 0 ? 0 : position.x < size.x - 1 ? 1 : 2;
            var y = position.y == 0 ? 0 : position.y < size.y - 1 ? 1 : 2;

            return new Vector2Int(x, y);
        }
    }
}