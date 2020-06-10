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

        //実寸/10で定義する
        internal float laneLength = 0.7f;
        public float[] highSpeed = {8.0f, 8.0f};

        [Serializable]
        public class NoteTimingInfomation
        {
            public NoteTimingInfomation(int _noteType, float _reachTime)
            {
                this.noteType = _noteType;
                this.reachTime = _reachTime;
            }

            public int noteType;
            public float reachTime;
        }

        public List<NoteTimingInfomation>[,] timings = new List<NoteTimingInfomation>[2, 5];
        public List<NoteTimingInfomation>[,] moreEasyTimings = new List<NoteTimingInfomation>[2, 5];
        public List<NoteTimingInfomation>[,] moreDifficultTimings = new List<NoteTimingInfomation>[2, 5];


        private void Awake()
        {
            
            if (SceneLoadManager.Instance.previewSceneTransitionData is MusicSelectManager.MusicData musicData)
            {
                _musicData = musicData;
            }
            else
            {
                _musicData = new MusicSelectManager.MusicData();

            }

            Debug.Log($"MusicDaaaaaaaaaaaaaaaaaata:{_musicData}");
            for (int i = 0; i < PlayerManager.Instance._players.Length; i++)
            {
                highSpeed[i] = _musicData.HISPEED[i];
                Debug.Log("Player " + i + " is Hi-Speed" + highSpeed[i]);
            }


            //初期化
            for (int i = 0; i < 2; i++)
            {
                mainNotes[i] = new List<NoteObject>();
                moreEasyNotes[i] = new List<NoteObject>();
                moreDifficultNotes[i] = new List<NoteObject>();

                for (int k = 0; k < 5; k++)
                {
                    timings[i, k] = new List<NoteTimingInfomation>();
                    moreEasyTimings[i, k] = new List<NoteTimingInfomation>();
                    moreDifficultTimings[i, k] = new List<NoteTimingInfomation>();
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

            var path = "Musics/" + _musicData.musicID;
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