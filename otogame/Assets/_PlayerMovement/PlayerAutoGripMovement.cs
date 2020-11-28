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

    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            InputFunction(inputMovementTime, i, timings[i],
                _keyInputManager.PlayerManager.players[PlayerId].FumenState);
        }
    }

    protected override bool InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;

        //現在の楽曲再生時間
        var inputTime = inputMovementTime;

        if (targetTimings.Count == _cacheNoteCounters[stateIndex, targetLane])
        {
            return false;
        }

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_cacheNoteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo._reachTime;

        //ここでノーツの種類判定と化すればそれぞれのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo._noteType;
        if (noteType == 5)
        {
            if (0.025f > GetDifferentAbs(inputTime, judgeTime))
            {
                GripLaneLight();
                var valid = base.InputFunction(inputMovementTime, targetLane, targetTimings, fumenState);
                if (valid)
                {
                    _keyInputManager.PlayerManager.OnUseOtofudaCard(PlayerId, true);
                }
            }
        }

        return true;
    }


    private void GripLaneLight()
    {
        _keyInputManager.keyBeamController.BeamOnAll();
        /*for (int i = 0; i < _keyInputManager.laneLight.Length; i++)
        {
            _keyInputManager.laneLight[i].SetActive(true);
        }*/


        if (_cacheReleaseTapCoroutine != null)
        {
            StopCoroutine(_cacheReleaseTapCoroutine);
        }

        _cacheReleaseTapCoroutine = StartCoroutine(ReleaseGripCoroutine());
    }

    private IEnumerator ReleaseGripCoroutine()
    {
        yield return _releaseWait;
        _keyInputManager.keyBeamController.BeamOffAll();
        /*for (int i = 0; i < _keyInputManager.laneLight.Length; i++)
        {
            _keyInputManager.laneLight[i].SetActive(false);
        }*/
    }
}