using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerTapMovement : PlayerMovement
{
    //単純なタップの判定
    //あとでこいつにインデックスを渡してforの回数を減らす
    public override void PlayerMovementCheck()
    {
        if (!Input.anyKeyDown) return;
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            if (!Input.GetKeyDown(PlayerKeys[i])) continue;
            InputFunction(i, _inputManager._fumenDataManager.timings[PlayerId, i],
                _inputManager._playerManager._players[PlayerId].FumenState);



            //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
/*                        switch (PlayerManager._players[PlayerId].FumenState)
                        {
                            case PlayerFumenState.DEFAULT:
                                _judger.KeyJudge(i, _inputManager._fumenDataManager.timings[PlayerId, i], PlayerFumenState.DEFAULT);
                                break;
                            case PlayerFumenState.MORE_DIFFICULT:
                                _judger.KeyJudge(i, _inputManager._fumenDataManager.moreDifficultTimings[PlayerId, i],
                                    PlayerFumenState.MORE_DIFFICULT);
                                break;
                            default:
                                break;
                        }*/
        }

    }

    public override void InputFunction(int targetLane, List<FumenDataManager.NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        base.InputFunction(targetLane, targetTimings, fumenState);
    }

    public override PlayerKeyInputManager.Judge InputJudge(float inputTime, float judgeTime, int targetLane, int noteType, int stateIndex)
    {
        if (noteType == 1 || noteType == 2)
        {
            //Debug.Log(base.InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex));
            return base.InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
        }
        else
        {
            return PlayerKeyInputManager.Judge.None;
        }
    }
}
