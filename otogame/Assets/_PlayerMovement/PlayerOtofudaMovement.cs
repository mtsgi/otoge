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


    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        //Debug.Log("てーすと:"+_receivedGripState);
        if (_getPlayerGripGesture._PlayerGripState != PlayerGripState.RELEASE)
        {
            _receivedGripState = _getPlayerGripGesture._PlayerGripState;

            InputFunction(inputMovementTime, 2, timings[2],
                _keyInputManager.PlayerManager.players[PlayerId].FumenState);
            /*InputFunction(2, _keyInputManager._fumenDataManager.defaultTimings[PlayerId, 2],
                _keyInputManager._playerManager._players[PlayerId].FumenState);*/
        }
    }


    private void OnGetPlayerGripGesture(int playerId, PlayerGripState gripState)
    {
        if (playerId != PlayerId) return;

        _receivedGripState = gripState;

        if (_receivedGripState == PlayerGripState.RELEASE)
        {
            _keyInputManager.keyBeamController.BeamOffAll();
            /*foreach (var t in _keyInputManager.laneLight)
            {
                t.SetActive(false);
//                Debug.Log("おふった");
            }*/
        }
        else if (_receivedGripState == PlayerGripState.GRIP)
        {
            _keyInputManager.keyBeamController.BeamOnAll();
            /*foreach (var t in _keyInputManager.laneLight)
            {
                t.SetActive(true);
             //   Debug.Log("おんった");
            }*/
        }
    }

    protected override bool InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;
        if (_cacheNoteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
            return false;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_cacheNoteCounters[stateIndex, targetLane]];

        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var inputTime = inputMovementTime;
        var judgeTime = nextNoteTimingInfo._reachTime;
        var noteType = nextNoteTimingInfo._noteType;


        if (nextNoteTimingInfo._noteEntity.IsActive == false)
        {
            return false;
        }

        //入力の精度の判定
        var judgeResult = InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);

        _tmpNoteType = nextNoteTimingInfo._noteType;


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

            //音札のアクティベート関数を実行
            _keyInputManager.PlayerManager.OnUseOtofudaCard(PlayerId, judgeResult == Judge.Perfect);
        }

        return true;
    }


    public override Judge InputJudge(float inputTime, float judgeTime, int targetLane,
        int noteType, int stateIndex)
    {
        if (_tmpNoteType == 5)
        {
            if (inputTime - judgeTime < -BadThreshold)
            {
                return Judge.None;
            }

            if (_receivedGripState == PlayerGripState.GRIP &&
                -BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
            {
                /*
                _noteCounters[stateIndex, targetLane]++;
                */
                return Judge.Perfect;
            }
        }

        return Judge.None;
    }
}