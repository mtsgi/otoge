using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace shigeno_EditorUtility
{
    public class NonEditableAttribute : PropertyAttribute
    {
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NonEditableAttribute))]
    public class NonEditableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            NonEditableAttribute NonEditable = (NonEditableAttribute) attribute;

            EditorGUI.BeginDisabledGroup(true);

            EditorGUI.PropertyField(position, property, label);

            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}