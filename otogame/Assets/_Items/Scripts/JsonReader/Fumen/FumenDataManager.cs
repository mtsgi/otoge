using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenDataManager : SingletonMonoBehaviour<FumenDataManager>
    {
        public bool isDebug = false;
        public MusicDataObject debugMusicData;

        internal List<NoteObject>[] mainNotes = new List<NoteObject>[2];
        internal List<NoteObject>[] moreEasyNotes = new List<NoteObject>[2];
        internal List<NoteObject>[] moreDifficultNotes = new List<NoteObject>[2];
        [SerializeField] private FumenFlowManager[] _fumenFlowManager;

        public MusicData _musicData = new MusicData();
        private PlayerInfo[] _playerInfos = new PlayerInfo[2];

        //実寸/10で定義する
        internal float laneLength = 0.7f;
        public float[] _highSpeed = {8.0f, 8.0f};

        [Serializable]
        public class NoteTimingInformation
        {
            public NoteTimingInformation(int _noteType, float _reachTime)
            {
                this.noteType = _noteType;
                this.reachTime = _reachTime;
            }

            public int noteType;
            public float reachTime;
        }

        public List<NoteTimingInformation>[,] timings = new List<NoteTimingInformation>[2, 5];
        public List<NoteTimingInformation>[,] moreEasyTimings = new List<NoteTimingInformation>[2, 5];
        public List<NoteTimingInformation>[,] moreDifficultTimings = new List<NoteTimingInformation>[2, 5];


        private new void Awake()
        {
            base.Awake();
            Debug.Log("FumenDataManagerAwake");

            var fumenSelectSceneData =
                SceneLoadManager.Instance.previewSceneTransitionData as FumenSelectSceneTransitionData;

            Debug.Log($"{SceneLoadManager.Instance.testTypeName}");

            if (isDebug)
            {
                SetDebugData();
                _playerInfos = new PlayerInfo[] {new PlayerInfo(), new PlayerInfo()};
                Debug.Log("Test Mode");
            }
            else if (fumenSelectSceneData != null)
            {
                _musicData = fumenSelectSceneData.musicData;
                _playerInfos = fumenSelectSceneData.playerInfos;
            }

            _musicData.TestCheckParameter();

            //ハイスピ周りはPlayerManagerに渡したい。
            //というかこれMonoにしたくない
            for (int i = 0; i < 2; i++)
            {
                _highSpeed[i] = _playerInfos[i].hiSpeed;
            }
            

            //初期化
            for (int i = 0; i < 2; i++)
            {
                mainNotes[i] = new List<NoteObject>();
                moreEasyNotes[i] = new List<NoteObject>();
                moreDifficultNotes[i] = new List<NoteObject>();

                for (int k = 0; k < 5; k++)
                {
                    timings[i, k] = new List<NoteTimingInformation>();
                    moreEasyTimings[i, k] = new List<NoteTimingInformation>();
                    moreDifficultTimings[i, k] = new List<NoteTimingInformation>();
                }
            }
            
        }

        private void Start()
        {
            for (int i = 0; i < PlayerManager.Instance._players.Length; i++)
            {
                var jsonReader = new JsonReadManager(_musicData);
                jsonReader.Init(this, i);
            }

            var path = "Musics/" + _musicData.musicId;
            Debug.Log(path);
            var audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
            SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0].clip = audioClip;
            if (audioClip != null) SoundManager.Instance._soundListSettings[0].soundName = audioClip.name;
            SoundManager.Instance.initDictionary();
            StartCoroutine(FumenStartWait());
        }

        private IEnumerator FumenStartWait()
        {
            var waitSec = (60 / _musicData.bpm);
            var waitForBlankRhythm = new WaitForSeconds(waitSec);
            yield return new WaitForSeconds(2);
            SoundManager.Instance.playSound(1).Play();
            yield return waitForBlankRhythm;
            SoundManager.Instance.playSound(1).Play();
            yield return waitForBlankRhythm;
            SoundManager.Instance.playSound(1).Play();
            yield return waitForBlankRhythm;

            for (int i = 0; i < 2; i++)
            {
                _fumenFlowManager[i].StartFumenFlow();
            }
        }

        private void SetDebugData()
        {
            _musicData = debugMusicData.GetMusicData();
        }
    }
}