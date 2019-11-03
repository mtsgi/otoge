using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.player;
using UnityEngine;

public class PlayerOtofudaMovement : PlayerMovement
{

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
        if (GetPlayerGripGesture._PlayerGripState != PlayerGripState.RELEASE)
        {
            _receivedGripState = GetPlayerGripGesture._PlayerGripState;
            
            InputFunction(2, _inputManager._fumenDataManager.timings[PlayerId, 2],
                _inputManager._playerManager._players[PlayerId].FumenState);
        }
        
    }


    private void OnGetPlayerGripGesture(int _playerId, PlayerGripState _gripState)
    {
        if (_playerId == PlayerId)
        {
            _receivedGripState = _gripState;
        }
        
        if (_receivedGripState == PlayerGripState.RELEASE)
        {
            foreach (var t in _inputManager.laneLight)
            {
                t.SetActive(false);
             //   Debug.Log("おふった");
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

    public override void InputFunction(int targetLane, List<FumenDataManager.NoteTimingInfomation> targetTimings,
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
        
        if (_inputManager._noteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
            return;
        }
        

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager._noteCounters[stateIndex, targetLane]];

        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var inputTime = _inputManager._audioSource.time;
        var judgeTime = nextNoteTimingInfo.reachTime;
        var noteType = nextNoteTimingInfo.noteType;

        _tmpNoteType = nextNoteTimingInfo.noteType;
            
        var judgeResult = InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
            
        
        if (judgeResult != PlayerKeyInpuManager.Judge.None)
        {
            _inputManager.judgeTextAnimators[(int) judgeResult].Play("Judge", 0, 0.0f);
        }
        else
        {
            //判定がNoneだったばあい、そのノーツは処理していないのでretする
            return;
        }

        Debug.Log("残りノーツ数: "+_inputManager._noteCounters[stateIndex, targetLane]);
        
        if (stateIndex == 1)
        {
            _inputManager._fumenDataManager.mainNotes[PlayerId].RemoveAt(0);
        }
        else if (stateIndex == 2)
        {
            _inputManager._fumenDataManager.moreDifficultNotes[PlayerId].RemoveAt(0);
        }
        
        
    }


    public override PlayerKeyInpuManager.Judge InputJudge(float inputTime, float judgeTime, int targetLane, int noteType, int stateIndex)
    {
        if (_tmpNoteType == 5)
        {
            if (inputTime - judgeTime < -BadThreshold)
            {
                return PlayerKeyInpuManager.Judge.None;
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
                _inputManager._noteCounters[stateIndex, targetLane]++;
                _inputManager._playerManager._players[PlayerId].noteSimpleCount++;
                Debug.Log("ぱふぇ！");
                return PlayerKeyInpuManager.Judge.Perfect;
            }
        }

        return PlayerKeyInpuManager.Judge.None;
        
    }
}
