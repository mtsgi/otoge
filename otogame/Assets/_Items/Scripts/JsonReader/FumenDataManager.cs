using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class FumenDataManager : SingletonMonoBehaviour<FumenDataManager>
{
    internal float BPM = 120.0f;
    internal List<Note> mainNotes = new List<Note>();
    internal float laneLength = 0.03f;

}
