using Essentials;
using UnityEngine;

namespace CameraSystems
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Transforms a CameraTransformation from local to world space
        /// </summary>
        public static CameraTransformation Transform(this Transform t, CameraTransformation transformation)
        {
            var position = t.TransformPoint(transformation.position);
            var transformEulerAngles = t.rotation.eulerAngles;
            var originalEulerAngles = transformation.eulerAngles;
            var eulerAngles = new Vector3(
                originalEulerAngles.x,
                MathUtil.Wrap(originalEulerAngles.y + transformEulerAngles.y, 360),
                originalEulerAngles.z
            );
            return new CameraTransformation()
            {
                position = position,
                eulerAngles = eulerAngles,
                distance = transformation.distance,
                fov = transformation.fov
            };
        }

        /// <summary>
        /// Transforms a CameraTransformation from world to local space
        /// </summary>
        public static CameraTransformation InverseTransform(this Transform t, CameraTransformation transformation)
        {
            var position = t.InverseTransformPoint(transformation.position);
            var transformEulerAngles = t.rotation.eulerAngles;
            var originalEulerAngles = transformation.eulerAngles;
            var eulerAngles = new Vector3(
                originalEulerAngles.x,
                MathUtil.Wrap(originalEulerAngles.y + transformEulerAngles.y, 360),
                originalEulerAngles.z
            );
            return new CameraTransformation()
            {
                position = position,
                eulerAngles = eulerAngles,
                distance = transformation.distance,
                fov = transformation.fov
            };
        }
    }
}