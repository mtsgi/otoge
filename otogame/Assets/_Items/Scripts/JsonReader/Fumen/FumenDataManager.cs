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

        public float BPM = 120.0f;
        internal float BEAT = 4.0f;
        internal List<NoteObject>[] mainNotes = new List<NoteObject>[2];
        internal List<NoteObject>[] moreEasyNotes = new List<NoteObject>[2];
        internal List<NoteObject>[] moreDifficultNotes = new List<NoteObject>[2];
        [SerializeField] private FumenFlowManager[] _fumenFlowManager;

        MusicSelectManager.MusicData _musicData = new MusicSelectManager.MusicData();
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
            if (fumenSelectSceneData != null)
            {
                _musicData = fumenSelectSceneData.musicData;
                _playerInfos = fumenSelectSceneData.playerInfos;
            }
            else
            {
                _musicData = new MusicSelectManager.MusicData();
                _playerInfos = new PlayerInfo[]{new PlayerInfo(), new PlayerInfo()};
            }

            
            //ハイスピ周りはPlayerManagerに渡したい。
            //というかこれMonoにしたくない
            for (int i = 0; i < 2; i++)
            {
                _highSpeed[i] = _playerInfos[i].hiSpeed;
            }

/*            for (int i = 0; i < PlayerManager.Instance._players.Length; i++)
            {
                _highSpeed[i] = PlayerManager.Instance.;
                Debug.Log("Player " + i + " is Hi-Speed" + highSpeed[i]);
            }*/


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

/*
            Debug.Log("timings is "+timings.Length);
*/

/*            for (int i = 0; i < 10; i++)
            {
                timings[i] = new List<NoteTimingInfomation>();
            }*/
        }

        private void Start()
        {
/*            SoundManager.Instance.gameObject.GetComponent<AudioSource>().clip =
                Resources.Load("Musics/+" + IndexJsonReadManager.musicID, typeof(AudioClip)) as AudioClip;*/

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
            var waitSec = (60 / BPM);
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
    }
}