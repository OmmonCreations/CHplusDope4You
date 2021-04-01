using UnityEngine;

namespace AnimatedObjects.Transforms
{
    public abstract class ToggleableTransformController : ToggleableObjectController
    {
        [SerializeField] private Transform _transform = null;

        public Transform Transform => _transform;
    }
}