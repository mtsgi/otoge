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

    
    
    public override void PlayerMovementCheck()
    {
        if (GetPlayerFlickGesture._playerGesture != PlayerGesture.NONE)
        {
            _receiveGesture = GetPlayerFlickGesture._playerGesture;
            
            for (int i = 0; i < PlayerKeys.Length; i++)
            {
                //if (!_inputManager.isLongNoteStart[i]) continue;
            
                InputFunction(i, _inputManager._fumenDataManager.timings[PlayerId, i],
                    _inputManager._playerManager._players[PlayerId].FumenState);
            }
        }
        
/*
        //アクション発火を受け取る必要ないのでは？
        //離したとき
        if (recieveGesture == PlayerGesture.NONE) return false;
        for (int i = 0; i < PlayerKeys.Length; i++)
        {
            if (Event.current.keyCode != PlayerKeys[i]) continue;
            if (!_inputManager.isLongNoteStart[i]) continue;
            
            InputFunction(i, _inputManager._fumenDataManager.timings[PlayerId, i],
                _inputManager._playerManager._players[PlayerId].FumenState);
                        
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


    public override void InputFunction(int targetLane, List<FumenDataManager.NoteTimingInformation> targetTimings,
        PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;

        //もうリストがなくなりきってたらはじく
        if ((stateIndex == 1 && _inputManager._fumenDataManager.mainNotes[PlayerId].Count == 0) ||
            (stateIndex == 2 && _inputManager._fumenDataManager.moreDifficultNotes[PlayerId].Count == 0))
        {
            return;
        }

        if (!_inputManager.isStartMusic)
        {
            return;
        }
        
        if (_inputManager.noteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
//            Debug.Log("かぞえきったよ");
            return;
        }
        

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager.noteCounters[stateIndex, targetLane]];

        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var inputTime = _inputManager._audioSource.time;
        var judgeTime = nextNoteTimingInfo.reachTime;
        var noteType = nextNoteTimingInfo.noteType;

        _tmpNoteType = nextNoteTimingInfo.noteType;
            
        var judgeResult = InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
            
        if (judgeResult != PlayerKeyInputManager.Judge.None)
        {
            if (judgeResult == PlayerKeyInputManager.Judge.Perfect || judgeResult == PlayerKeyInputManager.Judge.Good)
            {
                _inputManager.ComboUp();
            }
            else if (judgeResult == PlayerKeyInputManager.Judge.Bad)
            {
                _inputManager.ComboCut();
            }
            
            _inputManager.judgeTextAnimators[(int) judgeResult].Play("Judge", 0, 0.0f);
        }
        else
        {
            //判定がNoneだったばあい、そのノーツは処理していないのでretする
            return;
        }
            
        if (stateIndex == 1)
        {
            _inputManager._fumenDataManager.mainNotes[PlayerId][0].DeleteNote();
            _inputManager._fumenDataManager.mainNotes[PlayerId].RemoveAt(0);
        }
        else if (stateIndex == 2)
        {
            _inputManager._fumenDataManager.moreDifficultNotes[PlayerId][0].DeleteNote();
            _inputManager._fumenDataManager.moreDifficultNotes[PlayerId].RemoveAt(0);
        }
        
    }


    public override PlayerKeyInputManager.Judge InputJudge(float inputTime, float judgeTime, int targetLane, int noteType, int stateIndex)
    {
        //チュウニみたいなガバ判定にしています
        if (_tmpNoteType == 3)
        {
            if (-BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
            {
                _inputManager.noteCounters[stateIndex, targetLane]++;
                _inputManager.noteSimpleCount++;

                if (_receiveGesture == PlayerGesture.LEFT)
                {
                    return PlayerKeyInputManager.Judge.Perfect;
                }

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
                _inputManager.noteCounters[stateIndex, targetLane]++;
                _inputManager.noteSimpleCount++;

                if (_receiveGesture == PlayerGesture.RIGHT)
                {
                    return PlayerKeyInputManager.Judge.Perfect;
                }
/*                else if (_recieveGesture == PlayerGesture.LEFT)
                {
                    return PlayerKeyInpuManager.Judge.Bad;
                }*/
            }
        }

        return PlayerKeyInputManager.Judge.None;

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
