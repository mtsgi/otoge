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
        internal float laneLength = 0.03f;

    }
}
