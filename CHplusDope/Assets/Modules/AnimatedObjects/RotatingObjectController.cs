using System;
using UnityEngine;

namespace AnimatedObjects
{
    public class RotatingObjectController : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private Vector3 _rotation = Vector3.up;

        private void Update()
        {
            _transform.localEulerAngles += _rotation * Time.deltaTime;
        }
    }
}