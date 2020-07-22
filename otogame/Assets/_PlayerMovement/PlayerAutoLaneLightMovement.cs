using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerAutoLaneLightMovement : PlayerMovement
{
    public override void PlayerMovementCheck()
    {
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            InputFunction(i, _inputManager._fumenDataManager.timings[PlayerId, i],
                _inputManager._playerManager._players[PlayerId].FumenState);
        }
    }

    public override void InputFunction(int targetLane, List<FumenDataManager.NoteTimingInformation> targetTimings,
        PlayerFumenState fumenState)
    {
        /*var stateIndex = (int) fumenState;

        //現在の楽曲再生時間
        var inputTime = _inputManager._audioSource.time;

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager._noteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo.reachTime;

        //ここでノーツの種類判定と化すればそれぞれのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo.noteType;
        if (noteType == 1 || noteType == 2)
        {
            if (0.02f > GetDifferentAbs(inputTime, judgeTime))
            {
                base.InputFunction(targetLane, targetTimings, fumenState);
            }
        }*/
    }
}
