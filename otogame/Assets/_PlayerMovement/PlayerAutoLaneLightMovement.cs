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


    public override void PlayerMovementCheck()
    {
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            InputFunction(i, _inputManager._fumenDataManager.timings[PlayerId, i],
                _inputManager._playerManager._players[PlayerId].FumenState);
        }
    }

    public override void InputFunction(int targetLane, List<FumenDataManager.NoteTimingInformation> targetTimings,
        PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;
        //現在の楽曲再生時間
        var inputTime = _inputManager._audioSource.time;

        if (targetTimings.Count == _inputManager.noteCounters[stateIndex, targetLane])
        {
            return;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager.noteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo.reachTime;

        //ここでノーツの種類判定と化すればそれぞ}れのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo.noteType;

        if (0.025f > GetDifferentAbs(inputTime, judgeTime))
        {
            if (noteType == 1 || noteType == 3 || noteType == 4)
            {
                //ロングの終端ノーツである場合はlaneLightを消すのみ
                if (_inputManager.isLongNoteStart[targetLane])
                {
                    _inputManager.laneLight[targetLane].SetActive(false);
                }
                else
                {
                    TapLaneLight(targetLane);
                }
            }
            else if (noteType == 2)
            {
                //ロングの始点ノーツである場合はlaneLightを表示する
                _inputManager.laneLight[targetLane].SetActive(true);
            }
        }
    }

    private void TapLaneLight(int lane)
    {
        _inputManager.laneLight[lane].SetActive(true);

        if (_cacheReleaseTapCoroutines[lane] != null)
        {
            StopCoroutine(_cacheReleaseTapCoroutines[lane]);
        }

        _cacheReleaseTapCoroutines[lane] = StartCoroutine(ReleaseTapCoroutine(lane));
    }

    private IEnumerator ReleaseTapCoroutine(int targetLane)
    {
        yield return _releaseWait;
        _inputManager.laneLight[targetLane].SetActive(false);
    }
}