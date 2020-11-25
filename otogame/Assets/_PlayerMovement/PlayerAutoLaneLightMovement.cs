using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerAutoLaneLightMovement : PlayerMovement
{
    [SerializeField] private float releaseTapWait = 0.03f;
    private Coroutine[] _cacheReleaseTapCoroutines;
    private WaitForSeconds _releaseWait;

    public override void InitMovement(PlayerKeyInputManager playerKeyInputManager)
    {
        base.InitMovement(playerKeyInputManager);
        _releaseWait = new WaitForSeconds(releaseTapWait);
        _cacheReleaseTapCoroutines = new Coroutine[PlayerKeys.Length];
    }


    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            InputFunction(inputMovementTime, i, timings[i],
                _keyInputManager.PlayerManager._players[PlayerId].FumenState);
        }
    }

    protected override void InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;
        //現在の楽曲再生時間
        var inputTime = inputMovementTime;

        if (targetTimings.Count == _cacheNoteCounters[stateIndex, targetLane])
        {
            return;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_cacheNoteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo._reachTime;

        //ここでノーツの種類判定と化すればそれぞ}れのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo._noteType;

        if (0.025f > GetDifferentAbs(inputTime, judgeTime))
        {
            if (noteType == 1 || noteType == 3 || noteType == 4)
            {
                //ロングの終端ノーツである場合はlaneLightを消すのみ
                if (_keyInputManager.isLongNoteStart[targetLane])
                {
                    _keyInputManager.keyBeamController.BeamOffAt(targetLane);
//                    _keyInputManager.laneLight[targetLane].SetActive(false);
                }
                else
                {
                    TapLaneLight(targetLane);
                }
            }
            else if (noteType == 2)
            {
                //ロングの始点ノーツである場合はlaneLightを表示する
                _keyInputManager.keyBeamController.BeamOnAt(targetLane);
                /*_keyInputManager.laneLight[targetLane].SetActive(true);*/
            }
        }
    }

    private void TapLaneLight(int lane)
    {
        _keyInputManager.keyBeamController.BeamOnAt(lane);
//        _keyInputManager.laneLight[lane].SetActive(true);

        if (_cacheReleaseTapCoroutines[lane] != null)
        {
            StopCoroutine(_cacheReleaseTapCoroutines[lane]);
        }

        _cacheReleaseTapCoroutines[lane] = StartCoroutine(ReleaseTapCoroutine(lane));
    }

    private IEnumerator ReleaseTapCoroutine(int targetLane)
    {
        yield return _releaseWait;
        _keyInputManager.keyBeamController.BeamOffAt(targetLane);
//        _keyInputManager.laneLight[targetLane].SetActive(false);
    }
}