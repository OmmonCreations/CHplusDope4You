#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

namespace Switches
{
    [CustomEditor(typeof(Switch))]
    public class SwitchEditor : ToggleEditor
    {
        private SerializedProperty _knobProperty;
        private SerializedProperty _onColorsProperty;
        private SerializedProperty _offColorsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _knobProperty = serializedObject.FindProperty("_knob");
            _onColorsProperty = serializedObject.FindProperty("_onColors");
            _offColorsProperty = serializedObject.FindProperty("_offColors");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(_knobProperty);
            EditorGUILayout.PropertyField(_onColorsProperty);
            EditorGUILayout.PropertyField(_offColorsProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif