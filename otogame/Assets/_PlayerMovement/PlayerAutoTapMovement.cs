using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerAutoTapMovement : PlayerMovement
{
    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            InputFunction(inputMovementTime, i, timings[i],
                _keyInputManager.PlayerManager._players[PlayerId].FumenState);
        }
    }

    protected override void InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;

        //現在の楽曲再生時間
        var inputTime = inputMovementTime;

        if (targetTimings.Count == _cacheNoteCounters[stateIndex, targetLane])
        {
            return;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_cacheNoteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo._reachTime;

        //ここでノーツの種類判定と化すればそれぞれのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo._noteType;
        if (0.025f > GetDifferentAbs(inputTime, judgeTime))
        {
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

                base.InputFunction(inputMovementTime, targetLane, targetTimings, fumenState);
            }
        }
    }
}