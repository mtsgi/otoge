using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

interface INoteJudge
{
    void KeyJudge(int targetLane, List<NoteTimingInformation> targetTimings,
        PlayerFumenState fumenState);
}