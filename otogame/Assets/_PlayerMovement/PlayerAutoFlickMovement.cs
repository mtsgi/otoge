using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerAutoFlickMovement : PlayerMovement
{
    [SerializeField] private HandPositionTracer tracer;

    private Vector3 defaultPos;
    private Vector3 flickPos;
    private float flickDistance;

    private void Start()
    {
        defaultPos = tracer.transform.position;
        flickPos = defaultPos;
        flickPos.x += 0.5f;

        flickDistance = Vector3.Distance(defaultPos, flickPos);
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
        if (_inputManager.laneLight[targetLane].activeSelf)
        {
            _inputManager.laneLight[targetLane].SetActive(false);
        }

        var stateIndex = (int) fumenState;

        //現在の楽曲再生時間
        var inputTime = _inputManager._audioSource.time;

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager._noteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo.reachTime;

        //ここでノーツの種類判定と化すればそれぞれのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo.noteType;
        if (noteType == 3 || noteType == 4)
        {
            if (0.05f > GetDifferentAbs(inputTime, judgeTime))
            {
                if (noteType == 3)
                {
                    float present_Location = (Time.time * 1.0f) / flickDistance;
                    tracer.transform.position = Vector3.Lerp(defaultPos, flickPos, present_Location);
                }
                else
                {
                    float present_Location = (Time.time * 1.0f) / flickDistance;
                    tracer.transform.position = Vector3.Lerp(defaultPos, flickPos, -present_Location);
                }


                _inputManager.laneLight[targetLane].SetActive(true);

                base.InputFunction(targetLane, targetTimings, fumenState);
            }
        }
    }

    private float GetDifferentAbs(float value1, float value2)
    {
        return Mathf.Abs(value1 - value2);
    }
}