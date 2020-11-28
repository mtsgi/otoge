using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerTapMovement : PlayerMovement
{
    //単純なタップの判定
    //あとでこいつにインデックスを渡してforの回数を減らす
    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        if (!Input.anyKeyDown) return;
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            if (!Input.GetKeyDown(PlayerKeys[i])) continue;
            InputFunction(inputMovementTime, i, timings[i],
                _keyInputManager.PlayerManager.players[PlayerId].FumenState);
        }
    }

    protected override bool InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        return base.InputFunction(inputMovementTime, targetLane, targetTimings, fumenState);
    }

    public override Judge InputJudge(float inputTime, float judgeTime, int targetLane,
        int noteType, int stateIndex)
    {
        /*Debug.Log($"Tap {inputTime - judgeTime}");*/
        if (noteType == 1 || noteType == 2)
        {
            if (noteType == 1)
            {
                _keyInputManager.isLongNoteStart[targetLane] = false;
            }

            if (noteType == 2)
            {
                _keyInputManager.isLongNoteStart[targetLane] = true;
            }

            //Debug.Log(base.InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex));
            return base.InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
        }
        else
        {
            return Judge.None;
        }
    }
}