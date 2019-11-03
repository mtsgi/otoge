using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerTapMovement : PlayerMovement
{
    //単純なタップの判定
    public override void PlayerMovementCheck()
    {
        if (Event.current.type != EventType.KeyDown) return;
        if (!Input.GetKeyDown(Event.current.keyCode)) return;
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            if (Event.current.keyCode != PlayerKeys[i]) continue;
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
    
}
