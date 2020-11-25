﻿using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class NoteJudger : MonoBehaviour, INoteJudge
{
    public virtual void KeyJudge(int targetLane, List<NoteTimingInformation> targetTimings,
        PlayerFumenState fumenState)
    {
    }
}