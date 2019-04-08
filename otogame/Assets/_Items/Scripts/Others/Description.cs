﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using shigeno_EditorUtility;

public class Description : MonoBehaviour
{
    [TextArea(1,6)]
    public string comment = "ここにスクリプトとかの説明文を書いてください\n";

    [CustomLabel("コメントを書いた人")] 
    public string name = "ななし";

}

[CustomEditor (typeof(Description))]
public class DescriptionInspector : Editor
{
    Description _description = null;

    void OnEnable ()
    {
        //Character コンポーネントを取得
        _description = (Description) target;
    }

    public override void OnInspectorGUI ()
    {
        EditorGUILayout.LabelField ("スクリプトの説明を書いたりする用です");
        base.OnInspectorGUI ();
    }
}
