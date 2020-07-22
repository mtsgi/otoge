using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerAutoFlickMovement : PlayerMovement
{
    [SerializeField] private HandPositionTracer tracer;

    private Vector3 _defaultPos;
    private Vector3 _flickPosR;
    private Vector3 _flickPosL;
    
    private Coroutine _flickSimulateCoroutine;

    private void Start()
    {
        _defaultPos = tracer.transform.position;
        _flickPosR = _flickPosL = _defaultPos;

        _flickPosR.x += 0.5f;
        _flickPosL.x -= 0.5f;
        
    }

    private void Update()
    {
        Test();
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

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager._noteCounters[stateIndex, targetLane]];
        var judgeTime = nextNoteTimingInfo.reachTime;

        //ここでノーツの種類判定と化すればそれぞれのエフェクトとかの実行ができそう
        var noteType = nextNoteTimingInfo.noteType;
        if (noteType == 3 || noteType == 4)
        {
            if (0.02f > GetDifferentAbs(inputTime, judgeTime))
            {
                if (noteType == 3)
                {
                    SimulateFlick(_flickPosL);
                }
                else
                {
                    SimulateFlick(_flickPosR);
                }

                base.InputFunction(targetLane, targetTimings, fumenState);
            }
        }
    }

    private void Test()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            SimulateFlick(_flickPosL);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            SimulateFlick(_flickPosR);
        }
    }

    private void SimulateFlick(Vector3 flickTargetPos)
    {
        if (_flickSimulateCoroutine != null)
        {
            StopCoroutine(_flickSimulateCoroutine);
        }

        _flickSimulateCoroutine =
            StartCoroutine(FlickHandTraceWithLinearInterpolate(tracer.transform, 0.2f, flickTargetPos));
    }


    private IEnumerator FlickHandTraceWithLinearInterpolate(Transform tracerTransform, float time, Vector3? position)
    {
        var isReturn = false;

        // 現在のposition
        var currentPosition = tracerTransform.position;
        // 目標のposition
        var targetPosition = position ?? currentPosition;

        var sumTime = 0f;
        while (true)
        {
            // Coroutine開始フレームから何秒経過したか
            sumTime += Time.deltaTime;
            // 指定された時間に対して経過した時間の割合
            var ratio = sumTime / time;

            if (!isReturn)
            {
                tracerTransform.position = Vector3.Lerp(currentPosition, targetPosition, ratio);

                if (ratio > 1.0f)
                {
                    // ~.Lerpは割合を示す引数は0 ~ 1の間にClampされるので1より大きくても問題なし
                    currentPosition = tracerTransform.position;
                    sumTime = 0.0f;
                    isReturn = true;
                }
            }
            else
            {
                tracerTransform.position = Vector3.Lerp(currentPosition, _defaultPos, ratio);
                if (ratio > 1.0f)
                {
                    break;
                }
            }
            
            yield return null;
        }
    }
}