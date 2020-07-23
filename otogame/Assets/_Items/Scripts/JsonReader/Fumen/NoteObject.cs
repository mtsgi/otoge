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

        private bool isPlayingGame;

        private int frameConnt;

        private float defPosX;

        //Z座標はノーツの難易度が変化するとともに変わるのでinternalとかにしておく
        internal float posZ;

        private Stopwatch _stopwatch = new Stopwatch();

        private AudioSource _audioSource;

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

                transform.transform.position = new Vector3(defPosX,
                    (reachFrame - _audioSource.time) * _laneLength * _hiSpeed, posZ);
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
        public void SetNoteObject(int type, int lane, int end, float[] option, float reach, int playerId,
            float hiSpeed, float laneLength)
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
        }

        public void ChangeFumenState()
        {
//           Debug.Log("stateChange");
            isPlayingGame = !isPlayingGame;
            _stopwatch.Start();
            _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];
//            Debug.Log($"AAAAAAAAAA{_laneLength}");
        }

        public void DeleteNote()
        {
            transform.gameObject.SetActive(false);
        }
    }
}