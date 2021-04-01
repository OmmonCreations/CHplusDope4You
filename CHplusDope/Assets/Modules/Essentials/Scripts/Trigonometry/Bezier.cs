using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Trigonometry
{
    [System.Serializable]
    public class Bezier : System.Object
    {
        //vars to store our control points
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;

        private BezierCheckpoint[] _checkpoints;
        private float _totalLengthSquared;
        private float _totalLength;

        public float Length
        {
            get { return _totalLength; }
        }

        public bool IsStraight
        {
            get
            {
                var handleA = (p1 - p0).normalized;
                var handleB = (p3 - p2).normalized;
                var handleC = (p3 - p0).normalized;
                return Vector3.Dot(handleA, handleB) >= 0.99f &&
                                 Vector3.Dot(handleA, handleC) >= 0.99f;
            }
        }

        // Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
        public Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.p0 = v0;
            this.p1 = v1;
            this.p2 = v2;
            this.p3 = v3;
        }

        public GameObject Draw(float time = 10f)
        {
            if (_checkpoints == null) CalculateCheckpoints(100);
            List<Vector3> positions = new List<Vector3>();
            foreach (BezierCheckpoint p in _checkpoints)
            {
                positions.Add(p.position);
            }

            GameObject result = new GameObject("Bezier");
            BezierDrawer drawer = result.AddComponent<BezierDrawer>();
            drawer.Bezier = this;
            Object.Destroy(result, time);
            return result;
        }

        public void DrawGizmos()
        {
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawSphere(p0, 0.025f);
            Gizmos.DrawWireSphere(p1, 0.05f);
            Gizmos.DrawWireSphere(p2, 0.05f);
            Gizmos.DrawSphere(p3, 0.025f);
            for (int i = 0; i < _checkpoints.Length - 1; i++)
            {
                Gizmos.DrawLine(_checkpoints[i].position, _checkpoints[i + 1].position);
            }
        }

        public void CalculateCheckpoints(int precision = 100)
        {
            _checkpoints = new BezierCheckpoint[precision + 1];
            _totalLength = 0;
            for (var i = 0; i <= precision; i++)
            {
                var position = Evaluate((float) i / precision);
                float distance;
                if (i == 0) distance = 0;
                else distance = (position - _checkpoints[i - 1].position).magnitude;
                _checkpoints[i] = new BezierCheckpoint(position, distance);
                _totalLength += distance;
            }

            if (_totalLength <= 0)
            {
                Debug.Log("Bezier length is 0!");
                _checkpoints = null;
                return;
            }

            _totalLengthSquared = Mathf.Pow(_totalLengthSquared, 2);

            float passedLength = 0;
            for (int i = 0; i < _checkpoints.Length; i++)
            {
                passedLength += _checkpoints[i].distance;
                _checkpoints[i].t = passedLength / _totalLength;
            }
        }

        public Vector3 Evaluate(float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; //first term
            p += 3 * uu * t * p1; //second term
            p += 3 * u * tt * p2; //third term
            p += ttt * p3; //fourth term

            return p;
        }

        public Vector3 EvaluateLinear(float t)
        {
            if(_checkpoints==null) CalculateCheckpoints();
            
            if (t <= 0)
            {
                return _checkpoints[0].position;
            }

            for (int i = 0; i < _checkpoints.Length; i++)
            {
                BezierCheckpoint b = _checkpoints[i];
                if (i == 0 || b.t < t) continue;
                BezierCheckpoint a = _checkpoints[i - 1];
                var remainingTime = t - a.t;
                var progress = _totalLength * remainingTime;
                return Vector3.MoveTowards(a.position, b.position, progress);
            }

            return _checkpoints[_checkpoints.Length - 1].position;
        }
    }
}