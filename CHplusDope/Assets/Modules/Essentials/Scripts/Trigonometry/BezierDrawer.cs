using UnityEngine;

namespace Essentials.Trigonometry
{
    public class BezierDrawer : MonoBehaviour
    {
        public Bezier Bezier;

        private void OnDrawGizmos()
        {
            Bezier.DrawGizmos();
        }
    }
}