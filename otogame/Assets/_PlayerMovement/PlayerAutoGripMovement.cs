using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerAutoGripMovement : PlayerMovement
{
    [SerializeField] private float releaseTapWait = 0.03f;
    private Coroutine _cacheReleaseTapCoroutine;
    private WaitForSeconds _releaseWait;

    public override void InitMovement(PlayerKeyInputManager playerKeyInputManager)
    {
        base.InitMovement(playerKeyInputManager);
        _releaseWait = new WaitForSeconds(releaseTapWait);
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

        if (targetTimings.Count == _inputManager._noteCounters[stateIndex, targetLane])
        {
            return;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager._noteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo.reachTime;

        //ここでノーツの種類判定と化すればそれぞれのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo.noteType;
        if (noteType == 5)
        {
            if (0.025f > GetDifferentAbs(inputTime, judgeTime))
            {
                GripLaneLight();
                base.InputFunction(targetLane, targetTimings, fumenState);
            }
        }
    }

    private void GripLaneLight()
    {
        for (int i = 0; i < _inputManager.laneLight.Length; i++)
        {
            _inputManager.laneLight[i].SetActive(true);
        }


        if (_cacheReleaseTapCoroutine != null)
        {
            StopCoroutine(_cacheReleaseTapCoroutine);
        }

        _cacheReleaseTapCoroutine = StartCoroutine(ReleaseGripCoroutine());
    }

    private IEnumerator ReleaseGripCoroutine()
    {
        yield return _releaseWait;
        for (int i = 0; i < _inputManager.laneLight.Length; i++)
        {
            _inputManager.laneLight[i].SetActive(false);
        }
    }
}