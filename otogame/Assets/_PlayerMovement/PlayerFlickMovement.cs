using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.player;
using UnityEngine;

public class PlayerFlickMovement : PlayerMovement
{
    private PlayerGesture _receiveGesture;

    //フリックの判定を持つために一時的に代入しておく変数
    private int _tmpNoteType;

    private void OnEnable()
    {
        GetPlayerFlickGesture.OnGetPlayerGesture += OnGetPlayerGesture;
    }


    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        if (GetPlayerFlickGesture._playerGesture != PlayerGesture.NONE)
        {
            _receiveGesture = GetPlayerFlickGesture._playerGesture;

            for (int i = 0; i < PlayerKeys.Length; i++)
            {
                InputFunction(inputMovementTime, i, timings[i],
                    _keyInputManager.PlayerManager._players[PlayerId].FumenState);
            }
        }

/*
        //アクション発火を受け取る必要ないのでは？
        //離したとき
        if (recieveGesture == PlayerGesture.NONE) return false;
        for (int i = 0; i < PlayerKeys.Length; i++)
        {
            if (Event.current.keyCode != PlayerKeys[i]) continue;
            if (!_keyInputManager.isLongNoteStart[i]) continue;
            
            InputFunction(i, _keyInputManager._fumenDataManager.timings[PlayerId, i],
                _keyInputManager._playerManager._players[PlayerId].FumenState);
                        
            //入力があったらtrue
            return true;
            
        }*/
    }


    private void OnGetPlayerGesture(int _playerId, PlayerGesture _gesture)
    {
        if (_playerId == PlayerId)
        {
            _receiveGesture = _gesture;
        }
    }


    protected override void InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;
        if (_cacheNoteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
            return;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_cacheNoteCounters[stateIndex, targetLane]];

        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var inputTime = inputMovementTime;
        var judgeTime = nextNoteTimingInfo._reachTime;
        var noteType = nextNoteTimingInfo._noteType;

        _tmpNoteType = nextNoteTimingInfo._noteType;

        
        if (nextNoteTimingInfo._noteEntity.IsActive == false)
        {
            return;
        }
        
        //入力の精度の判定
        var judgeResult = InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
//        Debug.Log(judgeResult);
        if (judgeResult != Judge.None)
        {
            if (judgeResult == Judge.Perfect ||
                judgeResult == Judge.Good)
            {
                _keyInputManager.ComboUp(judgeResult);
            }
            else if (judgeResult == Judge.Bad)
            {
                _keyInputManager.ComboCut(Judge.Bad);
            }

            //判定後
            //NoteObjectをディアクティベートする。
            nextNoteTimingInfo._noteEntity.Deactivate(judgeResult);
        }
    }


    public override Judge InputJudge(float inputTime, float judgeTime, int targetLane,
        int noteType, int stateIndex)
    {
        //チュウニみたいなガバ判定にしています
        if (_tmpNoteType == 3)
        {
            if (-BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
            {
                /*_noteCounters[stateIndex, targetLane]++;*/
                //_keyInputManager.noteSimpleCount++;

                if (_receiveGesture == PlayerGesture.LEFT)
                {
                    return Judge.Perfect;
                }

                //これをつけると巻き込みとかの判定が厳しくなりすぎるのでダメ
/*                else if (_recieveGesture == PlayerGesture.RIGHT)
                {
                    return PlayerKeyInpuManager.Judge.Bad;
                }*/
            }
        }
        else if (_tmpNoteType == 4)
        {
            if (-BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
            {
                /*_noteCounters[stateIndex, targetLane]++;*/
                //_keyInputManager.noteSimpleCount++;

                if (_receiveGesture == PlayerGesture.RIGHT)
                {
                    return Judge.Perfect;
                }

/*                else if (_recieveGesture == PlayerGesture.LEFT)
                {
                    return PlayerKeyInpuManager.Judge.Bad;
                }*/
            }
        }

        return Judge.None;
    }


/*    if (_gesture != PlayerGesture.NONE)
    {
//                Debug.Log("GetsGesture!" + ":" + _gesture);
        for (int i = 0; i < 5; i++)
        {
            //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
            switch (_playerManager._players[playerID].FumenState)
            {
                case PlayerFumenState.DEFAULT:
                    checkJudgeFlickNote(_gesture, _fumenDataManager.timings[_playerId, i],
                        PlayerFumenState.DEFAULT);
                    break;
                case PlayerFumenState.MORE_DIFFICULT:
                    checkJudgeFlickNote(_gesture, _fumenDataManager.moreDifficultTimings[_playerId, i],
                        PlayerFumenState.MORE_DIFFICULT);
/*
                            checkSingleJudge(i, _fumenDataManager.timings[playerID, i], PlayerFumenState.MORE_DIFFICULT);
#1#
                    break;
                default:
                    break;
            }                 
                    
        }*/
/*
                checkJudgeFlickNote(_gesture);
*/
    //}
}