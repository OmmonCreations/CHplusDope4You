using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

namespace GyroCameras
{
    public class GyroController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform = null;

        private AttitudeSensor _attitudeSensor;
        private Gyroscope _gyroscope;

        protected void Start()
        {
            var attitudeSensor = AttitudeSensor.current;
            var gyroscope = Gyroscope.current;

            if (attitudeSensor != null)
            {
                Debug.Log("Enabling attitude sensor");
                InputSystem.EnableDevice(attitudeSensor);
            }
            else if (gyroscope != null)
            {
                Debug.Log("Enabling gyro");
                InputSystem.EnableDevice(gyroscope);
            }
            else
            {
                Debug.LogError("No attitude sensor and no gyroscope found.");
            }

            _attitudeSensor = attitudeSensor;
            _gyroscope = gyroscope;
            enabled = attitudeSensor != null || gyroscope != null;
        }

        private void OnDestroy()
        {
            var attitudeSensor = _attitudeSensor;
            var gyroscope = _gyroscope;

            if (attitudeSensor != null) InputSystem.DisableDevice(attitudeSensor);
            if (gyroscope != null) InputSystem.DisableDevice(gyroscope);
        }

        protected void Update()
        {
            if (_attitudeSensor != null) UpdateAttitude();
            else if (_gyroscope != null) UpdateGyro();
        }

        private void UpdateAttitude()
        {
            _cameraTransform.rotation = GyroToUnity(_attitudeSensor.attitude.ReadValue());
        }

        private void UpdateGyro()
        {
            _cameraTransform.rotation *= GyroToUnity(Quaternion.Euler(-_gyroscope.angularVelocity.ReadValue()));
        }
        
        private static Quaternion GyroToUnity(Quaternion q)
        {
            var euler = q.eulerAngles;
            return Quaternion.Euler(euler.x, euler.y, -euler.z);
        }
    }
}