using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OtoFuda.RythmSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace OtoFuda.Fumen
{
    //ノーツ本体を定義するスクリプト

    //BPMは別から参照

    //BPMは一秒間の拍数。
    //n/n拍子の時
    public class NoteObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color deactivateColor = new Color(255, 255, 255, 0.5f);
        public int noteType = 0;
        public int lane = 0;
        public int playerID;

        public List<NotesInfo> endNotes = new List<NotesInfo>();

        //endnoteが全体で何個目のノーツかを格納
        private int endNotesNum;

        public int _endNotesNum
        {
            get { return endNotesNum; }
            set { endNotesNum = value; }
        }

        public float[] option = null;
        public float reachFrame = 0.0f;

        private float _laneLength;
        public float test;
        public float _hiSpeed = 1.0f;

        public int _measure;

        private bool isPlayingGame;

        private int frameConnt;

        private float defPosX;

        //Z座標はノーツの難易度が変化するとともに変わるのでinternalとかにしておく
        internal float posZ;

        //private Stopwatch _stopwatch = new Stopwatch();

        private AudioSource _audioSource;


        public List<NoteObject> childNote = null;

        private bool isActive;
        public bool IsActive => isActive;

        public bool isRun;

        private void Start()
        {
/*            _highSpeed = FumenDataManager.Instance.highSpeed[playerID];
            _laneLength = FumenDataManager.Instance.laneLength;*/
            defPosX = transform.position.x;
            posZ = transform.position.z;
        }

        private void LateUpdate()
        {
            //Debug.Log($"{_laneLength}");
            if (isPlayingGame)
            {
                test = _laneLength;

                if (isRun)
                {
                    transform.transform.position = new Vector3(defPosX,
                        (reachFrame - _audioSource.time) * _laneLength * _hiSpeed, posZ);
                }

/*              frameConnt++;
              Debug.Log(frameConnt);
              if (frameConnt == 60)
              {
                  frameConnt = 0;
                  Debug.Log("Count！");
              }*/
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lane"></param>
        /// <param name="end"></param>
        /// <param name="option"></param>
        /// <param name="reach"></param>
        /// <param name="playerId"></param>
        /// <param name="hiSpeed"></param>
        /// <param name="laneLength"></param>
        /// <param name="measure"></param>
        public void SetNoteObject(int type, int lane, int end, float[] option, float reach, int playerId,
            float hiSpeed, float laneLength, int measure)
        {
            noteType = type;
            this.lane = lane;
            endNotes = new List<NotesInfo>();

            endNotesNum = end;

            this.option = option;
            reachFrame = reach;
            playerID = playerId;

            _hiSpeed = hiSpeed;
            _laneLength = laneLength;
            test = _laneLength;

            _measure = measure;
        }

        public void ChangeFumenState()
        {
//           Debug.Log("stateChange");
            isPlayingGame = !isPlayingGame;

            if (isPlayingGame)
            {
                Activate();
            }
            else
            {
                Deactivate(Judge.None);
            }

            //_stopwatch.Start();
            //todo 関数から渡せるようにする
            _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];
//            Debug.Log($"AAAAAAAAAA{_laneLength}");
        }

        public void Activate()
        {
            isActive = true;
            isRun = true;
            if (_renderer != null)
            {
                _renderer.color = defaultColor;
            }
        }

        public void Deactivate(Judge judge)
        {
            isActive = false;

            if (_renderer != null)
            {
                _renderer.color = deactivateColor;
            }

            if (judge == Judge.Bad || judge == Judge.Miss)
            {
                //始点ノーツをミスしたら終点ノーツもディアクティベート
                if (childNote.Count > 0)
                {
                    for (int i = 0; i < childNote.Count; i++)
                    {
                        Debug.Log($"{i} 番目の子ノーツをDeactivateしました");
                        childNote[i].Deactivate(judge);
                    }
                }
            }
        }

        public void StopRun()
        {
            isRun = false;
        }

        public void DeleteNote()
        {
            transform.gameObject.SetActive(false);
        }
    }
}