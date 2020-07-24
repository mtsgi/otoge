#if UNITY_EDITOR
using OtoFuda.RythmSystem;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SoundManager))]
public class SoundManagerInspector : Editor
{
    SoundManager _soundManager = null;

    void OnEnable()
    {
        //Character コンポーネントを取得
        _soundManager = (SoundManager) target;
    }

    //拡張インスペクタ
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        _soundManager.testPlayAudioClipName =
            EditorGUILayout.TextField("登録名からサウンドを再生するテスト", _soundManager.testPlayAudioClipName);
        if (GUILayout.Button("SoundTest By Name"))
        {
            soundTestByName(_soundManager.testPlayAudioClipName);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _soundManager.testPlayAudioClipIdx =
            EditorGUILayout.IntField("インデックスからサウンドを再生するテスト", _soundManager.testPlayAudioClipIdx);
        if (GUILayout.Button("SoundTest By Index"))
        {
            soundTestByIndex(_soundManager.testPlayAudioClipIdx);
        }

        EditorGUILayout.EndHorizontal();
    }


    //SoundListSettingの名前から再生するテスト
    private void soundTestByName(string soundName)
    {
        _soundManager.initDictionary();
        //辞書に含まれるかどうかを調べる
        if (!_soundManager.soundListDictionary.ContainsKey(soundName))
        {
            Debug.LogError("辞書に登録されていない名前のAudioClipを再生しようとしました。処理を中止します。");
            return;
        }

        var playAudioSource = _soundManager.playSound(soundName);
        playAudioSource.Play();
        Debug.Log(soundName + "を再生しました");
    }

    //SoundListSettingのインデックスから再生するテスト    
    private void soundTestByIndex(int soundIndex)
    {
        if (soundIndex < 0 || _soundManager._soundListSettings.Count <= soundIndex)
        {
            Debug.LogError("配列参照外エラーが起こりそうだったので処理を中断します");
            return;
        }

        var playAudioSource = _soundManager.playSound(soundIndex);
        playAudioSource.Play();
        Debug.Log(_soundManager._soundListSettings[soundIndex].soundName + "を再生しました");
    }
}
#endif