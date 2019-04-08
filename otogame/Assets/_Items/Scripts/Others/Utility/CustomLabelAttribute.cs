#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace shigeno_EditorUtility
{
    public class CustomLabelAttribute : PropertyAttribute
    {
        public string newFieldLabel;

        public CustomLabelAttribute(string _newLabel)
        {
            this.newFieldLabel = _newLabel;
        }
    }

    [CustomPropertyDrawer(typeof(CustomLabelAttribute))]
    public class CustomLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CustomLabelAttribute customLabel = (CustomLabelAttribute) attribute;
            label.text = customLabel.newFieldLabel;
            EditorGUI.PropertyField(position, property, label);
        }
    }

}

#endif