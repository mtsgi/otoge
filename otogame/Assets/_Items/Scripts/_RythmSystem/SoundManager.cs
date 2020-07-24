using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using shigeno_EditorUtility;
using UniRx.Async;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace OtoFuda.RythmSystem
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        [HideInInspector] public AudioSource[] _audioSources;
        internal Dictionary<string, AudioSource> soundListDictionary;


        //サウンドマネージャから呼び出せるサウンドの一覧
        [System.Serializable]
        public class SoundListSetting
        {
            [CustomLabel("サウンドの名前")] public string soundName;
            [CustomLabel("再生元になるAudioSource")] public AudioSource targetAudioSource;
            [NonEditable] public AudioClip targetAudioClip;
        }

        public List<SoundListSetting> _soundListSettings = new List<SoundListSetting>();


        [Header("AutoSetSettings/Test")] [CustomLabel("サウンド名も自動設定するか")] [SerializeField]
        private bool autoSetName = true;

        //テストボタンで実行するサウンドの名前
        internal string testPlayAudioClipName = "Registered name";

        //テストボタンで実行するサウンドのインデックス	
        internal int testPlayAudioClipIdx = 0;


        private void Awake()
        {
            base.Awake();
            InitDictionary();
        }


        //サウンド名で指定する場合
        internal AudioSource PlaySound(string targetSoundName)
        {
            return soundListDictionary[targetSoundName];
        }

        //インデックスで指定する場合
        internal AudioSource PlaySound(int targetSoundIdx)
        {
            return _soundListSettings[targetSoundIdx].targetAudioSource;
        }

        public void PlayAudioFromExternal(string path)
        {
            var t = LoadAudioFromExternal(path);
        }

        private async UniTask LoadAudioFromExternal(string path)
        {
            var extension = Path.GetExtension(path);
            var audioType = AudioType.WAV;
            Debug.Log($"Load Audio From {path}");
            Debug.Log($"Extension {extension}");

            switch (extension)
            {
                case ".wav":
                    audioType = AudioType.WAV;
                    break;
                case ".mp3":
                    audioType = AudioType.MPEG;
                    break;
                case ".ogg":
                    audioType = AudioType.OGGVORBIS;
                    break;
                default:
                    Debug.LogError($"Extension is mismatch {extension}");
                    return;
            }

            var r = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
            await r.SendWebRequest(); // UnityWebRequestをawaitできる
            var audioClip = ((DownloadHandlerAudioClip) r.downloadHandler).audioClip;

            /*var r = UnityWebRequest.Get(path);
            await r.SendWebRequest(); // UnityWebRequestをawaitできる
            var audioClip = r.downloadHandler.data;
            this.gameObject.GetComponents<AudioSource>()[0].clip.SetData(audioClip, audioClip.Length);*/

            this.gameObject.GetComponents<AudioSource>()[0].clip = audioClip;
        }


        //サウンドが登録された辞書を初期化する関数
        internal void InitDictionary()
        {
            //辞書を初期化
            soundListDictionary = new Dictionary<string, AudioSource>();

            //辞書にSoundListSettingsの内容を格納
            for (int i = 0; i < _soundListSettings.Count; i++)
            {
                soundListDictionary.Add(_soundListSettings[i].soundName, _soundListSettings[i].targetAudioSource);
            }
        }


        //自動でSoundListSettingに追加するコンテキストメニュー
        [ContextMenu("AutoSet")]
        public void AutoSet()
        {
            //AudioSourceが一つもなければ中断する
            if (GetComponents<AudioSource>().Length == 0)
            {
                Debug.LogError("SoundManagerにAudioSouceが一つも設定されていません。自動設定を中断します");
                return;
            }

            var sources = GetComponents<AudioSource>();

            for (int i = 0; i < sources.Length; i++)
            {
                if (autoSetName)
                {
                    _soundListSettings[i].soundName = sources[i].clip.name;
                }
                else
                {
                    _soundListSettings[i].soundName = _soundListSettings[i].soundName;
                }

                _soundListSettings[i].targetAudioClip = sources[i].clip;
                _soundListSettings[i].targetAudioSource = sources[i];
            }

            //デバッグを非実行中にも行えるようにここでも辞書を初期化する
            InitDictionary();
        }
    }
}