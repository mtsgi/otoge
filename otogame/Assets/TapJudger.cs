using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class TapJudger : NoteJudger
{
    public override void KeyJudge(int targetLane, List<FumenDataManager.NoteTimingInformation> targetTimings,
        PlayerFumenState fumenState)
    {
        base.KeyJudge(targetLane, targetTimings, fumenState);
        
    }
}
