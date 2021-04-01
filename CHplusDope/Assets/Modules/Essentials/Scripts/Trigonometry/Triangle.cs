using UnityEngine;

namespace Essentials.Trigonometry
{
    public struct Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }
}