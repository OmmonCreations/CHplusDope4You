using System;
using UnityEngine;

namespace Essentials
{
    public static class MathUtil
    {
        /// <summary>
        /// Wraps value between 0 (inclusive) and step (exclusive) by adding step until value is greater or equal to 0
        /// and subtracting step until value is smaller than step.
        /// </summary>
        /// <returns>The wrapped value</returns>
        public static float Wrap(float value, float step)
        {
            while (value < 0) value += step;
            while (value >= step) value -= step;

            return value;
        }
        
        /// <summary>
        /// Wraps value between 0 (inclusive) and step (exclusive) by adding step until value is greater or equal to 0
        /// and subtracting step until value is smaller than step.
        /// </summary>
        /// <returns>The wrapped value</returns>
        public static int Wrap(int value, int step)
        {
            while (value < 0) value += step;
            while (value >= step) value -= step;

            return value;
        }

        public static bool IsInsideHollowCircle(Vector2 position, float outerRadiusSquared, float innerRadiusSquared)
        {
            return IsInsideCircle(position, outerRadiusSquared) && !IsInsideCircle(position, innerRadiusSquared);
        }

        public static bool IsInsideCircle(Vector2 position, float radiusSquared)
        {
            return position.sqrMagnitude <= radiusSquared;
        }

        private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            var cp1 = Vector3.Cross(b - a, p1 - a);
            var cp2 = Vector3.Cross(b - a, p2 - a);
            return Vector3.Dot(cp1, cp2) >= 0;
        }

        public static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            return (SameSide(p,a, b,c) && SameSide(p,b, a,c) && SameSide(p,c, a,b));
        }
        private static float Sign (Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public static bool PointInTriangle (Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
        
        // Given three colinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        private static Boolean onSegment(Vector2 p, Vector2 q, Vector2 r) 
        { 
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) && 
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y)) 
                return true; 
  
            return false; 
        } 
  
        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private static int Orientation(Vector2 p, Vector2 q, Vector2 r) 
        { 
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
            // for details of below formula. 
            var val = (q.y - p.y) * (r.x - q.x) - 
                      (q.x - p.x) * (r.y - q.y); 
  
            if (Math.Abs(val) < 0.0001f) return 0; // colinear 
  
            return (val > 0)? 1: 2; // clock or counterclock wise 
        } 
  
        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        public static bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2, bool allowEdgeCases = true) 
        { 
            // Find the four orientations needed for general and 
            // special cases 
            int o1 = Orientation(p1, q1, p2); 
            int o2 = Orientation(p1, q1, q2); 
            int o3 = Orientation(p2, q2, p1); 
            int o4 = Orientation(p2, q2, q1); 
  
            // General case 
            if (o1 != o2 && o3 != o4) 
                return true;

            if (!allowEdgeCases) return false;
  
            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1)) return true; 
  
            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1)) return true; 
  
            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2)) return true; 
  
            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2)) return true; 
  
            return false; // Doesn't fall in any of the above cases 
        }
    }
}