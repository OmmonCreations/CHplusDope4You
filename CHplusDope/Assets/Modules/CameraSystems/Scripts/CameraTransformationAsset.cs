using UnityEngine;

namespace CameraSystems
{
    [CreateAssetMenu(fileName = "CameraTransformation", menuName = "CameraSystems/Camera Transformation")]
    public class CameraTransformationAsset : ScriptableObject
    {
        [SerializeField] private CameraTransformation _value = new CameraTransformation()
        {
            distance = 10,
            fov = 50
        };

        #if UNITY_EDITOR
        public CameraTransformation value
        {
            get => _value;
            set => _value = value;
        }
        #else
        public CameraTransformation value => _value;
        #endif
    }
}