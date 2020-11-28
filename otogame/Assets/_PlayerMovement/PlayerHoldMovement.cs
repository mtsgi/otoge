using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerHoldMovement : PlayerMovement
{
    //ホールドを離したときの判定
    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        //離したとき
        for (int i = 0; i < PlayerKeys.Length; i++)
        {
            if (!Input.GetKeyUp(PlayerKeys[i])) continue;
            //ここでキービームをオフにしている
            _keyInputManager.keyBeamController.BeamOffAt(i);
            /*_inputManager.laneLight[i].SetActive(false);*/
            /*if (!_keyInputManager.isLongNoteStart[i]) continue;*/

            InputFunction(inputMovementTime, i, timings[i],
                _keyInputManager.PlayerManager.players[PlayerId].FumenState);

/*
                        //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
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
                        }
*/
        }
    }

    protected override bool InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        /*Debug.Log("CheckHold");*/
        var valid = base.InputFunction(inputMovementTime, targetLane, targetTimings, fumenState);
        if (valid)
        {
            _keyInputManager.keyBeamController.BeamOffAt(targetLane);
        }

        return valid;
    }

    public override Judge InputJudge(float inputTime, float judgeTime, int targetLane, int noteType, int stateIndex)
    {
        /*Debug.Log($"Hold {inputTime - judgeTime}");*/
        return base.InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
    }
}