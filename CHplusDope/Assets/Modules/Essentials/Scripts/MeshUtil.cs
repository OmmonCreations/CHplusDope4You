using UnityEngine;

namespace Essentials
{
    public static class MeshUtil
    {
        public static int[] BuildTriangleIndices(int vertices, bool individualTriangles = false)
        {
            int[] result;
            if (individualTriangles)
            {
                var triangles = vertices / 3;
                result = new int[vertices];
                for (var i = 0; i < triangles; i++)
                {
                    result[i * 3 + 0] = i * 3 + 0;
                    result[i * 3 + 1] = i * 3 + 2;
                    result[i * 3 + 2] = i * 3 + 1;
                }
            }
            else
            {
                var triangles = vertices / 2 - 1;
                result = new int[triangles * 3];
                for (var i = 0; i < triangles; i ++)
                {
                    result[i * 3 + 0] = i + 0;
                    result[i * 3 + 1] = i + 2;
                    result[i * 3 + 2] = i + 1;
                }
            }

            return result;
        }
        
        public static int[] BuildQuadIndices(int vertices, bool individualQuads = false)
        {
            int[] result;
            if (individualQuads)
            {
                var quads = vertices / 4;
                result = new int[vertices];
                for (var i = 0; i < quads; i++)
                {
                    result[i * 4 + 0] = i * 4 + 0;
                    result[i * 4 + 1] = i * 4 + 2;
                    result[i * 4 + 2] = i * 4 + 3;
                    result[i * 4 + 3] = i * 4 + 1;
                }
            }
            else
            {
                var quads = vertices / 2 - 1;
                result = new int[quads * 4];
                for (var i = 0; i < quads; i ++)
                {
                    result[i * 4 + 0] = i * 2 + 0;
                    result[i * 4 + 1] = i * 2 + 2;
                    result[i * 4 + 2] = i * 2 + 3;
                    result[i * 4 + 3] = i * 2 + 1;
                }
            }

            return result;
        }

        public static Color[] BuildColors(Color color, int length)
        {
            var result = new Color[length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = color;
            }

            return result;
        }
    }
}