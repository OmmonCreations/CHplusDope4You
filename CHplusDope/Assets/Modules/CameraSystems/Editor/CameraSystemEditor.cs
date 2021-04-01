using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace CameraSystems.Editor
{
    [CustomEditor(typeof(CameraSystem))]
    public class CameraSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Copy Camera Transformation"))
            {
                var cameraSystem = (CameraSystem) target;
                var transformation = cameraSystem.CurrentTransform;
                EditorGUIUtility.systemCopyBuffer = transformation.Serialize().ToString();
            }
            if (GUILayout.Button("Paste Camera Transformation"))
            {
                var cameraSystem = (CameraSystem) target;
                var copyBuffer = EditorGUIUtility.systemCopyBuffer;
                JObject json;
                try
                {
                    json = JObject.Parse(copyBuffer);
                }
                catch
                {
                    Debug.LogWarning("Copy Buffer is empty.");
                    return;
                }

                if (!CameraTransformation.TryParse(json, out var t))
                {
                    Debug.LogWarning("Copy a camera transformation first.");
                    return;
                }

                cameraSystem.CurrentTransform = t;
            }
        }
    }
}
