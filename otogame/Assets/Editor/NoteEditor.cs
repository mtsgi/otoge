using System;
using Boo.Lang;
using OtoFuda.RythmSystem;
using UnityEngine;
using UnityEditor;
namespace akazukin_omochabako
{
    public class NoteEditor : EditorWindow
    {
        // SerializeFieldを定義
        [SerializeField]
        private List<int> _someList;

        [MenuItem("Window/Some Window")]
        private static void Open()
        {
            var window = GetWindow<NoteEditor>("Some Window");
        }
    
        private void OnGUI()
        {
            // 自身のSerializedObjectを取得
            var so = new SerializedObject(this);

            so.Update();
        
            // 第二引数をtrueにしたPropertyFieldで描画
            EditorGUILayout.PropertyField(so.FindProperty("_someList"), true);

            so.ApplyModifiedProperties();
        }
    }
}