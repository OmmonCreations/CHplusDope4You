using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Trigonometry
{
    [System.Serializable]
    public class BezierCurve
    {
        [SerializeField] private Bezier[] _parts;
	    private float _length = 0;
	    private bool _checkpointsCalculated = false;
	    
	    public Bezier[] Parts
	    {
		    get { return _parts; }
	    }
	    
	    public float Length
	    {
		    get { return _length; }
	    }

        public BezierCurve(params Bezier[] parts)
        {
            _parts = parts;
        }

	    public BezierCurve(params Vector3[] points)
	    {
		    List<Bezier> parts = new List<Bezier>();
		    for (int i = 0; i < points.Length - 1; i++)
		    {
			    Vector3 b = points[i];
			    Vector3 c = points[i + 1];
			    Vector3 a = i > 0 ? points[i - 1] : b - (c - b);
			    Vector3 d = (points.Length > i + 2) ? points[i + 2] : c - (b - c);

			    Vector3 ba = (a - b).normalized;
			    Vector3 bc = (c - b).normalized;
			    Vector3 abc = ba + bc;
			    Vector3 abcUp = Vector3.Cross(ba, bc).normalized;
			    
			    if(abc.sqrMagnitude <= 0) abc = Vector3.up;
			    if (abcUp.sqrMagnitude <= 0) abcUp = Vector3.Cross(ba, Vector3.up).normalized;
			    
			    Vector3 p1Handle = Vector3.Cross(abc, abcUp).normalized;

			    Vector3 cb = (b - c).normalized;
			    Vector3 cd = (d - c).normalized;
			    Vector3 bcd = cb + cd;
			    Vector3 bcdUp = Vector3.Cross(cb, cd).normalized;

			    if (bcd.sqrMagnitude <= 0) bcd = Vector3.up;
			    if (bcdUp.sqrMagnitude <= 0) bcdUp = Vector3.Cross(cb, Vector3.up).normalized;
			    
			    Vector3 p2Handle = Vector3.Cross(bcd,bcdUp).normalized;

			    float distance = (b - c).magnitude;
			    float handleLength = distance / 3;
			    Vector3 p0 = b;
			    Vector3 p1 = b - p1Handle * handleLength;
			    Vector3 p2 = c + p2Handle * handleLength;
			    Vector3 p3 = c;
			    parts.Add(new Bezier(p0,p1,p2,p3));
		    }
		    _parts = parts.ToArray();
	    }
        
        public GameObject Draw(float time = 10f)
		{
			if(!_checkpointsCalculated) CalculateCheckpoints(50);
			GameObject result = new GameObject("BezierCurve");
			foreach (Bezier bezier in _parts)
			{
				GameObject bezierObject = bezier.Draw(time);
				bezierObject.transform.SetParent(result.transform,true);
			}

			return result;
		}

		public void CalculateCheckpoints(int precision = 100)
		{
			_checkpointsCalculated = true;
			_length = 0;
			foreach (Bezier bezier in _parts)
			{
				bezier.CalculateCheckpoints(precision);
				_length += bezier.Length;
			}
		}
	
		// 0.0 >= t <= 1.0 In here be dragons and magic
		public Vector3 GetPoint( float t )
		{
			if (_parts.Length == 0) return Vector3.one;
			if (t <= 0) return _parts[0].Evaluate(0);
			if (t >= 1) return _parts[_parts.Length - 1].Evaluate(1);
			if(!_checkpointsCalculated) CalculateCheckpoints(50);
			float passedDistance = t * _length;
			for (int i = 0; i < _parts.Length; i++)
			{
				if (_parts[i].Length < passedDistance)
				{
					passedDistance -= _parts[i].Length;
					continue;
				}

				return _parts[i].Evaluate(passedDistance / _parts[i].Length);
			}
			return _parts[_parts.Length - 1].Evaluate(1);
		}
    }
}