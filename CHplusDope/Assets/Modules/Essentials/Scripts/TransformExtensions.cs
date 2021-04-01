using UnityEngine;

namespace Essentials
{
    public static class TransformExtensions
    {
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void Copy(this Transform transform, Transform other)
        {
            transform.position = other.position;
            transform.rotation = other.rotation;
            transform.localScale = other.localScale;
        }

        public static void Interpolate(this Transform transform, Transform a, Transform b, float t)
        {
            transform.position = Vector3.Lerp(a.position, b.position, t);
            transform.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
            transform.localScale = Vector3.one;
            
            var worldScale = Vector3.Lerp(a.lossyScale, b.lossyScale, t);
            var ownScale = transform.lossyScale;
            transform.localScale = new Vector3(worldScale.x / ownScale.x, worldScale.y / ownScale.y,
                worldScale.z / ownScale.z);
        }    
        
        public static string GetPath(this Transform t)
        {
            var parent = t.parent;
            return parent && parent != null ? parent.GetPath() + "/" + t.name : t.name;
        }
    }
}