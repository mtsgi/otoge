using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public NoteJudger _judger;

    internal int PlayerId;
    internal KeyCode[] PlayerKeys;

    protected float PerfectThreshold = 0.0f;
    protected float GoodThreshold = 0.0f;

    protected float BadThreshold = 0.0f;
    //internal float MissThreshold = 0.0f;

    protected int[,] _cacheNoteCounters;

    protected PlayerKeyInputManager _keyInputManager;
    private JudgeProfile _judgeProfile;

    public virtual void InitMovement(PlayerKeyInputManager keyInputManager)
    {
        _keyInputManager = keyInputManager;
        SetMovementValues();
        //  MissThreshold = playerKeyInputManager.judgeProfile.missThreshold;
    }

    private void SetMovementValues()
    {
        PlayerId = _keyInputManager.playerID;
        PlayerKeys = _keyInputManager.playerKeys;

        _cacheNoteCounters = _keyInputManager.CacheNoteCounters;

        //判定の閾値をProfileから引っ張ってくる
        _judgeProfile = _keyInputManager.JudgeProfile;

        PerfectThreshold = _judgeProfile.perfectThreshold;
        GoodThreshold = _judgeProfile.goodThreshold;
        BadThreshold = _judgeProfile.badThreshold;
    }


    public virtual void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
    }

    //入力のためのムーブメントを受け取った後のファンクション
    protected virtual bool InputFunction(float inputMovementTime, int targetLane,
        List<NoteTimingInformation> targetTimings, PlayerFumenState fumenState)
    {
        // fumenState
        //MORE_EASY = 0,
        //DEFAULT = 1,
        //MORE_DIFFICULT = 2,
        //End,
        var stateIndex = (int) fumenState;

        //todo _inputManager.noteCounters自体はInitでInputManagerから渡してあげてよくない？？？？？
        if (_cacheNoteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
            Debug.Log($"lane {targetLane} は かぞえきったよ");
            return false;
        }

        if (_cacheNoteCounters[stateIndex, targetLane] == targetTimings.Count)
        {
            Debug.Log($"lane {targetLane} は かぞえきったよ");
            return false;
        }

/*        //押下したキーに対応するレーンに流れるすべてのノーツ情報。
        わざわざ変数立てる必要あった？
        var targetLaneNoteInfos = targetTimings;*/

        //現在の次に来るはずのノーツ情報
        var nextNoteTimingInfo = targetTimings[_cacheNoteCounters[stateIndex, targetLane]];

        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var inputTime = inputMovementTime;
        var judgeTime = nextNoteTimingInfo._reachTime;
        var noteType = nextNoteTimingInfo._noteType;


        /*Debug.Log(
            $"next timing =>index:{_cacheNoteCounters[stateIndex, targetLane]},reachTime{nextNoteTimingInfo._reachTime},type:{nextNoteTimingInfo._noteEntity.noteType}");*/

        if (nextNoteTimingInfo._noteEntity.IsActive == false)
        {
            return false;
        }

        //入力の精度の判定
        var judgeResult = InputJudge(inputTime, judgeTime, targetLane, noteType, stateIndex);

//        Debug.Log(judgeResult);
        if (judgeResult != Judge.None)
        {
            if (judgeResult == Judge.Perfect ||
                judgeResult == Judge.Good)
            {
                //ロングの始点はコンボに加算しない
                if (noteType != 2 && noteType != 99)
                {
                    _keyInputManager.ComboUp(judgeResult);
                }

                /*_keyInputManager.ComboUp(judgeResult);*/
            }
            else if (judgeResult == Judge.Bad)
            {
                _keyInputManager.ComboCut(Judge.Bad);
            }

            //判定後
            //NoteObjectをディアクティベートする。
            nextNoteTimingInfo._noteEntity.Deactivate(judgeResult);
        }
        else
        {
//            Debug.Log("None!");
        }

        if (judgeResult != Judge.None)
        {
            CheckLongStartNote(targetLane, noteType);
            /* _noteCounters[stateIndex, targetLane]++; */
        }

        return true;
    }


    public virtual Judge InputJudge(float inputTime, float judgeTime, int targetLane,
        int noteType, int stateIndex)
    {
        var judgeResult = Judge.None;

        if (-PerfectThreshold <= inputTime - judgeTime && inputTime - judgeTime <= PerfectThreshold)
        {
            /*Debug.Log("perfe");*/
            judgeResult = Judge.Perfect;
        }
        else if (-GoodThreshold <= inputTime - judgeTime && inputTime - judgeTime <= GoodThreshold)
        {
            /*Debug.Log("good");*/
            judgeResult = Judge.Good;
        }
        else if (-BadThreshold <= inputTime - judgeTime && inputTime - judgeTime <= BadThreshold)
        {
            /*Debug.Log("bad");*/
            judgeResult = Judge.Bad;
        }
        else
        {
            /*Debug.Log("None");*/
        }


        return judgeResult;
    }

    //メモ
    //精度判定を行うときに
    //・現在譜面typeがそのノーツの譜面typeと一致していたら精度判定を行う
    //    していなければスルー精度判定ははじく
    //・ミス時は現在譜面typeがそのノーツの譜面typeと一致していたらmiss表示
    //    _inputManager._noteCounters[stateIndex, targetLane]++;は続行

    private void CheckLongStartNote(int lane, int type)
    {
        //isHaveChildNoteみたいな変数をノーツ生成時に持たせてあげることができれば、下フリックの直後に
        //ロングノーツを配置することもできるようになりそう。
        //具体的には_type==2で判定している部分をisHaveChildNote == trueであれば
        //(Json上でNoteの中にNoteが存在していれば)そのノーツはロングノーツの始点ノーツである。と判定ができる
        /*if (_fumenDataManager.timings[_lane][noteCount[_lane]].isHaveChildNote)
        {
            
        }*/

        //todo これ微妙すぎん？
        if (type == 2)
        {
//            Debug.Log("<color=yellow>StartLong</color>");
            _keyInputManager.isLongNoteStart[lane] = true;
        }
    }

    public float GetDifferentAbs(float value1, float value2)
    {
        return Mathf.Abs(value1 - value2);
    }
}