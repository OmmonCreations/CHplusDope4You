using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraSystems.Editor
{
    [CustomPropertyDrawer(typeof(CameraTransformation))]
    public class CameraTransformationPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var result = new VisualElement();

            // Create property fields.
            var positionField = new PropertyField(property.FindPropertyRelative("position"));
            var eulerAnglesField = new PropertyField(property.FindPropertyRelative("eulerAngles"));
            var fovField = new PropertyField(property.FindPropertyRelative("fov"), "Field Of View");
            var distanceField = new PropertyField(property.FindPropertyRelative("distance"), "Distance");

            // Add fields to the result.
            result.Add(positionField);
            result.Add(eulerAnglesField);
            result.Add(fovField);
            result.Add(distanceField);

            return result;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 5 * 25 + 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            var valuePosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            position.y += 20;
            position.x += 20;
            position.width -= 20;

            var y0 = position.y;
            var spacing = 5;
            var height = (position.height - 30) / 5 - spacing;
            var y1 = y0 + height + spacing;
            var y2 = y1 + height + spacing;
            var y3 = y2 + height + spacing;
            var y4 = y3 + height + spacing;
            var positionRect = new Rect(position.x, y0, position.width, height);
            var eulerAnglesRect = new Rect(position.x, y1, position.width, height);
            var fovRect = new Rect(position.x, y2, position.width, height);
            var distanceRect = new Rect(position.x, y3, position.width, height);
            var copyRect = new Rect(position.x, y4, position.width / 2, height);
            var pasteRect = new Rect(position.x + position.width / 2, y4, position.width / 2, height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("position"));
            EditorGUI.PropertyField(eulerAnglesRect, property.FindPropertyRelative("eulerAngles"));
            EditorGUI.PropertyField(fovRect, property.FindPropertyRelative("fov"));
            EditorGUI.PropertyField(distanceRect, property.FindPropertyRelative("distance"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            if (GUI.Button(copyRect, "Copy"))
            {
                Copy(property);
            }

            if (GUI.Button(pasteRect, "Paste"))
            {
                Paste(property);
            }

            EditorGUI.EndProperty();
        }

        private void Copy(SerializedProperty property)
        {
            var position = property.FindPropertyRelative("position").vector3Value;
            var eulerAngles = property.FindPropertyRelative("eulerAngles").vector3Value;
            var fov = property.FindPropertyRelative("fov").floatValue;
            var distance = property.FindPropertyRelative("distance").floatValue;

            var transformation = new CameraTransformation
            {
                position = position,
                eulerAngles = eulerAngles,
                fov = fov,
                distance = distance
            };

            EditorGUIUtility.systemCopyBuffer = transformation.Serialize().ToString();
        }

        private void Paste(SerializedProperty property)
        {
            var copyBuffer = EditorGUIUtility.systemCopyBuffer;
            var transformation = CameraTransformation.TryParse(copyBuffer, out var t) ? t : default;
            if (transformation.position == default &&
                transformation.eulerAngles == default &&
                Math.Abs(transformation.fov) <= 0 &&
                Math.Abs(transformation.distance) <= 0)
            {
                Debug.LogWarning("Copy a Camera Transformation first.");
                return;
            }

            property.FindPropertyRelative("position").vector3Value = transformation.position;
            property.FindPropertyRelative("eulerAngles").vector3Value = transformation.eulerAngles;
            property.FindPropertyRelative("fov").floatValue = transformation.fov;
            property.FindPropertyRelative("distance").floatValue = transformation.distance;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}