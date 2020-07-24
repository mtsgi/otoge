using System;
using System.IO.Ports;
using UnityEditor;
using UnityEngine;

namespace SerialPortUtility
{ 
    public enum BaudRate
    {
        B_300 = 300,
        B_1200 = 1200,
        B_2400 = 2400,
        B_4800 = 4800,
        B_9600 = 9600,
        B_19200 = 19200,
        B_38400 = 38400,
        B_57600 = 57600,
        B_74880 = 74880,
        B_115200 = 115200,
        B_230400 = 230400,
        B_250000 = 250000
    }

    [Serializable]
    public class SerialPortSetting
    {
        public BaudRate baudRate = BaudRate.B_57600;
        public int selectedPort;
        public string targetPortName;
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerialPortSetting))]
    public class SerialPortSettingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel++;
            position.height = EditorGUIUtility.singleLineHeight;
            var defaultPos = position;
            position.x = defaultPos.x;
            position.height = EditorGUIUtility.singleLineHeight;


            EditorGUI.PropertyField(position, property);
            if (!property.isExpanded) return;
            position.y += EditorGUIUtility.singleLineHeight;


            EditorGUI.LabelField(position, "ボーレート");
            position.x = defaultPos.width * 0.5f;
            var baudRateProperty = property.FindPropertyRelative("baudRate");
            baudRateProperty.enumValueIndex = EditorGUI.Popup(position, baudRateProperty.enumValueIndex,
                Enum.GetNames(typeof(BaudRate)));
            position.x = defaultPos.x;
            position.y += EditorGUIUtility.singleLineHeight;


            EditorGUI.LabelField(position, "シリアルポート");
            position.x = defaultPos.width * 0.5f;
            var selectedPortProperty = property.FindPropertyRelative("selectedPort");
            var names = SerialPort.GetPortNames();
            if (names.Length == 0)
            {
                names = new string[] {"No Serial Device"};
                selectedPortProperty.intValue = 0;
                selectedPortProperty.intValue =
                    EditorGUI.Popup(position, selectedPortProperty.intValue, names);
            }
            else
            {
                selectedPortProperty.intValue =
                    EditorGUI.Popup(position, selectedPortProperty.intValue, names);
            }

            var targetPortNameProperty = property.FindPropertyRelative("targetPortName");
            targetPortNameProperty.stringValue = names[selectedPortProperty.intValue];

            /*var targetPortNameProperty = property.FindPropertyRelative("targetPortName");
            targetPortNameProperty.stringValue = EditorGUI.TextField(position, targetPortNameProperty.stringValue);*/

            EditorGUI.indentLevel--;

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            return EditorGUIUtility.singleLineHeight * 3.5f;
        }
    }
#endif
}