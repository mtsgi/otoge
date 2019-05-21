using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Fumen
{
    [Serializable]
    public class FumenInfo
    {
        public List<NotesInfo> raku = null;
        public List<NotesInfo> easy = null;
        public List<NotesInfo> normal = null;
        public List<NotesInfo> hard = null;
        public List<NotesInfo> extra = null;
    }



    [Serializable]
    public class NotesInfo
    {
        public int type = 0;
        public int measure = 0;
        public int lane = 0;
        public int position = 0;
        public int split = 0;
        public int option = -1;
        public List<NotesInfo> end = null;
    }

}