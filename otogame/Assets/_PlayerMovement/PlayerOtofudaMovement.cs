using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.player;
using UnityEngine;

public class PlayerOtofudaMovement : PlayerMovement
{
    [SerializeField] private GetPlayerGripGesture _getPlayerGripGesture;
/*
    public static Action<int,bool> OnOtofudaUse;
*/

    private PlayerGripState _receivedGripState;
    //フリックの判定を持つために一時的に代入しておく変数
    private int _tmpNoteType;
    
    private void OnEnable()
    {
        GetPlayerGripGesture.OnGetPlayerGripGesture += OnGetPlayerGripGesture;
    }

    
    public override void PlayerMovementCheck()
    {
        //Debug.Log("てーすと:"+_receivedGripState);
        if (_getPlayerGripGesture._PlayerGripState != PlayerGripState.RELEASE)
        {
            _receivedGripState = _getPlayerGripGesture._PlayerGripState;
            
            InputFunction(2, _inputManager._fumenDataManager.timings[PlayerId, 2],
                _inputManager._playerManager._players[PlayerId].FumenState);
        }
        
    }


    private void OnGetPlayerGripGesture(int _playerId, PlayerGripState _gripState)
    {
        if (_playerId != PlayerId) return;
        
        _receivedGripState = _gripState;

        if (_receivedGripState == PlayerGripState.RELEASE)
        {
            foreach (var t in _inputManager.laneLight)
            {
                t.SetActive(false);
//                Debug.Log("おふった");
            }
        }
        else if (_receivedGripState == PlayerGripState.GRIP)
        {
            foreach (var t in _inputManager.laneLight)
            {
                t.SetActive(true);
             //   Debug.Log("おんった");
            }
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
            return;
        }
        
/*        Debug.Log(_inputManager._noteCounters[stateIndex, targetLane]);
        Debug.Log(targetLane + "___" + targetTimings[_inputManager._noteCounters[stateIndex, targetLane]]);*/
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
            //ここでいったん譜面の難易度を戻しておく
            _inputManager.judgeTextAnimators[(int) judgeResult].Play("Judge", 0, 0.0f);
        }
        else
        {
            //判定がNoneだったばあい、そのノーツは処理していないのでretする
            return;
        }

//        Debug.Log("残りノーツ数: "+_inputManager._noteCounters[stateIndex, targetLane]);


        //Debug.Log(_inputManager._fumenDataManager.mainNotes[PlayerId][0].noteType);
        if (_inputManager._fumenDataManager.mainNotes[PlayerId].Count != 0)
        {
            _inputManager._fumenDataManager.mainNotes[PlayerId][0].DeleteNote();
            _inputManager._fumenDataManager.mainNotes[PlayerId].RemoveAt(0);
        }


        //Debug.Log(_inputManager._fumenDataManager.moreDifficultNotes[PlayerId][0].noteType);
        if (_inputManager._fumenDataManager.moreDifficultNotes[PlayerId].Count != 0 )
        {
            _inputManager._fumenDataManager.moreDifficultNotes[PlayerId][0].DeleteNote();
            _inputManager._fumenDataManager.moreDifficultNotes[PlayerId].RemoveAt(0);
        }
        
/*        if (stateIndex == 1)
        {
            _inputManager._fumenDataManager.mainNotes[PlayerId].RemoveAt(0);

        }
        else if (stateIndex == 2)
        {
        }*/
        

        
        //音札のアクティベート関数を実行
        _inputManager._playerManager.OnUseOtofudaCard(PlayerId, judgeResult == PlayerKeyInputManager.Judge.Perfect);
//        Debug.Log("Invoke!");
    }


    public override PlayerKeyInputManager.Judge InputJudge(float inputTime, float judgeTime, int targetLane, int noteType, int stateIndex)
    {
        if (_tmpNoteType == 5)
        {
            if (inputTime - judgeTime < -BadThreshold)
            {
                return PlayerKeyInputManager.Judge.None;
            }
            
/*            if (inputTime - judgeTime < -MissThreshold)
            {
                Debug.Log("ばーか");

                return PlayerKeyInpuManager.Judge.Miss;
            }

            if (inputTime - judgeTime > BadThreshold)
            {
                return PlayerKeyInpuManager.Judge.None;
            }*/

            if (_receivedGripState == PlayerGripState.GRIP &&
                -BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
            {
                _inputManager.noteCounters[stateIndex, targetLane]++;
                _inputManager.noteSimpleCount++;
//                Debug.Log("ぱふぇ！");
                return PlayerKeyInputManager.Judge.Perfect;
            }
        }

        return PlayerKeyInputManager.Judge.None;
        
    }
}
