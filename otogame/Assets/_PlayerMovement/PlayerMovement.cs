using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public NoteJudger _judger;

    internal int PlayerId;
    internal KeyCode[] PlayerKeys;

    internal PlayerKeyInputManager _inputManager;


    internal float PerfectThreshold = 0.0f;
    internal float GoodThreshold = 0.0f;

    internal float BadThreshold = 0.0f;
    //internal float MissThreshold = 0.0f;


    public virtual void InitMovement(PlayerKeyInputManager playerKeyInputManager)
    {
        _inputManager = playerKeyInputManager;

        PlayerId = playerKeyInputManager.playerID;
        PlayerKeys = playerKeyInputManager.playerKeys;

        //判定の閾値をProfileから引っ張ってくる
        PerfectThreshold = playerKeyInputManager.judgeProfile.perfectThreshold;
        GoodThreshold = playerKeyInputManager.judgeProfile.goodThreshold;
        BadThreshold = playerKeyInputManager.judgeProfile.badThreshold;
        //  MissThreshold = playerKeyInputManager.judgeProfile.missThreshold;
    }

    /*プレイヤーのボタンの押下、フリック、ぎゅってするやつなどをチェックする関数
     ここをメインループで回す
    public virtual void PlayerMovementCheck(int index)
    {
    }
    の形にして全てのmovementでforが走らないようにするとパフォーマンス改善できそう
    かわらんか
    */


    public virtual void PlayerMovementCheck()
    {
    }

    //入力のためのムーブメントを受け取った後のファンクション
    public virtual void InputFunction(int targetLane,
        List<FumenDataManager.NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        var stateIndex = (int) fumenState;

        //もうリストがなくなりきってたらはじく
        if ((stateIndex == 1 && _inputManager._fumenDataManager.mainNotes[PlayerId].Count == 0) ||
            (stateIndex == 2 && _inputManager._fumenDataManager.moreDifficultNotes[PlayerId].Count == 0))
        {
//            Debug.Log("ないよ");
            return;
        }

        if (!_inputManager.isStartMusic)
        {
//            Debug.Log("はじまってないよ");
            return;
        }

        if (_inputManager.noteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
//            Debug.Log("かぞえきったよ");
            return;
        }

/*        //押下したキーに対応するレーンに流れるすべてのノーツ情報。
        わざわざ変数立てる必要あった？
        var targetLaneNoteInfos = targetTimings;*/

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_inputManager.noteCounters[stateIndex, targetLane]];

        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var inputTime = _inputManager._audioSource.time;
        var judgeTime = nextNoteTimingInfo.reachTime;
        var noteType = nextNoteTimingInfo.noteType;


        //入力の精度の判定
        var judgeResult = InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);
//        Debug.Log(judgeResult);
        if (judgeResult != PlayerKeyInputManager.Judge.None)
        {
            if (judgeResult == PlayerKeyInputManager.Judge.Perfect || judgeResult == PlayerKeyInputManager.Judge.Good)
            {
                //ロングの始点はコンボに加算しない
                if (noteType != 2 && noteType != 99)
                {
                    _inputManager.ComboUp();
                }
            }
            else if (judgeResult == PlayerKeyInputManager.Judge.Bad)
            {
                _inputManager.ComboCut();
            }

            _inputManager.judgeTextAnimators[(int) judgeResult].Play("Judge", 0, 0.0f);

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
        else
        {
            //判定がNoneだったばあい、そのノーツは処理していないのでretする
//            Debug.Log("すきっぷ");
        }
    }


    public virtual PlayerKeyInputManager.Judge InputJudge(float inputTime, float judgeTime, int targetLane,
        int noteType,
        int stateIndex)
    {
//        Debug.Log(inputTime - judgeTime);

        var judgeResult = PlayerKeyInputManager.Judge.None;

        if (-PerfectThreshold <= inputTime - judgeTime && inputTime - judgeTime <= PerfectThreshold)
        {
/*
            Debug.Log("perfe");
*/
            judgeResult = PlayerKeyInputManager.Judge.Perfect;
        }
        else if (-GoodThreshold <= inputTime - judgeTime && inputTime - judgeTime <= GoodThreshold)
        {
/*            Debug.Log("good");*/
            judgeResult = PlayerKeyInputManager.Judge.Good;
        }
        else if (-BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
        {
/*            Debug.Log("bad");*/
            judgeResult = PlayerKeyInputManager.Judge.Bad;
        }

        if (judgeResult != PlayerKeyInputManager.Judge.None)
        {
            CheckLongStartNote(targetLane, noteType);
            _inputManager.noteCounters[stateIndex, targetLane]++;
            _inputManager.noteSimpleCount++;
        }

        return judgeResult;
    }

    //メモ
    //精度判定を行うときに
    //・現在譜面typeがそのノーツの譜面typeと一致していたら精度判定を行う
    //    していなければスルー精度判定ははじく
    //・ミス時は現在譜面typeがそのノーツの譜面typeと一致していたらmiss表示
    //    _inputManager._noteCounters[stateIndex, targetLane]++;は続行

    private void CheckLongStartNote(int _lane, int _type)
    {
        //isHaveChildNoteみたいな変数をノーツ生成時に持たせてあげることができれば、下フリックの直後に
        //ロングノーツを配置することもできるようになりそう。
        //具体的には_type==2で判定している部分をisHaveChildNote == trueであれば
        //(Json上でNoteの中にNoteが存在していれば)そのノーツはロングノーツの始点ノーツである。と判定ができる
        /*if (_fumenDataManager.timings[_lane][noteCount[_lane]].isHaveChildNote)
        {
            
        }*/


        if (_type == 2)
        {
//            Debug.Log("<color=yellow>StartLong</color>");
            _inputManager.isLongNoteStart[_lane] = true;
        }
    }

    public float GetDifferentAbs(float value1, float value2)
    {
        return Mathf.Abs(value1 - value2);
    }
}