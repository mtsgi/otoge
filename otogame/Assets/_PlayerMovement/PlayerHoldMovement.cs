using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerHoldMovement : PlayerMovement
{
    //ホールドを離したときの判定
    public override void PlayerMovementCheck()
    {
        //離したとき
        if (Event.current.type != EventType.KeyUp) return;
        for (int i = 0; i < PlayerKeys.Length; i++)
        {
            if (Event.current.keyCode != PlayerKeys[i]) continue;
            //ここでキービームをオフにしている
            _inputManager.laneLight[i].SetActive(false);
            if (!_inputManager.isLongNoteStart[i]) continue;
            
            InputFunction(i, _inputManager._fumenDataManager.timings[PlayerId, i],
                _inputManager._playerManager._players[PlayerId].FumenState); 

/*                        //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
                        switch (_playerManager._players[playerID].FumenState)
                        {
                            case PlayerFumenState.DEFAULT:
                                checkSingleJudge(i, _fumenDataManager.timings[playerID, i], PlayerFumenState.DEFAULT);
                                break;
                            case PlayerFumenState.MORE_DIFFICULT:
                                checkSingleJudge(i, _fumenDataManager.moreDifficultTimings[playerID, i],
                                    PlayerFumenState.MORE_DIFFICULT);
                                break;
                            default:
                                break;
                        }*/
        }
    }

    public override void InputFunction(int targetLane, List<FumenDataManager.NoteTimingInfomation> targetTimings, PlayerFumenState fumenState)
    {
        base.InputFunction(targetLane, targetTimings, fumenState);
        _inputManager.laneLight[targetLane].SetActive(false);

    }
}
