using UnityEngine;

namespace Essentials.Trigonometry
{
    public struct BezierCheckpoint
    {
        public Vector3 position;
        public float distance;
        public float t;

        public BezierCheckpoint(Vector3 position, float distance)
        {
            this.position = position;
            this.distance = distance;
            this.t = 0;
        }
    }
}