using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenDataManager : SingletonMonoBehaviour<FumenDataManager>
    {
        public float BPM = 120.0f;
        internal float BEAT = 4.0f;
        internal List<NoteObject>[] mainNotes = new List<NoteObject>[2];
        internal List<NoteObject>[] moreEasyNotes = new List<NoteObject>[2];
        internal List<NoteObject>[] moreDifficultNotes = new List<NoteObject>[2];


        //実寸/10で定義する
        internal float laneLength = 0.7f;
        public float highSpeed = 8.0f;

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
        
        public List<NoteTimingInfomation>[,] timings = new List<NoteTimingInfomation>[2,5];
        public List<NoteTimingInfomation>[,] moreEasyTimings = new List<NoteTimingInfomation>[2,5];
        public List<NoteTimingInfomation>[,] moreDifficultTimings = new List<NoteTimingInfomation>[2,5];


        private void Awake()
        {
            //初期化
            for (int i = 0; i < 2; i++)
            {
                mainNotes[i]= new List<NoteObject>();
                moreEasyNotes[i]= new List<NoteObject>();
                moreDifficultNotes[i]= new List<NoteObject>();

                for (int k = 0; k < 5; k++)
                {
                    timings[i,k] = new List<NoteTimingInfomation>();
                    moreEasyTimings[i,k] = new List<NoteTimingInfomation>();
                    moreDifficultTimings[i,k] = new List<NoteTimingInfomation>();
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
    }
}
