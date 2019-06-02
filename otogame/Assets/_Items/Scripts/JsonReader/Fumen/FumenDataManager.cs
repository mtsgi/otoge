using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenDataManager : SingletonMonoBehaviour<FumenDataManager>
    {
        internal float BPM = 120.0f;
        internal float BEAT = 4.0f;
        internal List<NoteObject> mainNotes = new List<NoteObject>();
        //実寸/10で定義する
        internal float laneLength = 0.7f;
        internal float highSpeed = 8.0f;

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
        
        public List<NoteTimingInfomation>[] timings = new List<NoteTimingInfomation>[10];

        private void Awake()
        {
            //初期化
            for (int i = 0; i < 10; i++)
            {
                timings[i]=new List<NoteTimingInfomation>();
            }
        }
    }
}
