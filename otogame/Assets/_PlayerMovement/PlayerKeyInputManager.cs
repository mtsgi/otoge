using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKeyInputManager : FumenJudgeBehaviour
{
    internal bool isStartMusic = false;

    [Range(0, 1)] public int playerID;
    public KeyCode[] playerKeys = new KeyCode[5];

    //判定を表示する用のテキスト
    /*public Animator[] judgeTextAnimators;*/

    public KeyBeamController keyBeamController;

    //ロングノーツの開始をチェックしておくbool
    internal bool[] isLongNoteStart = new bool[5];

    //カードを使うときのアクション
    //パフェだったかのがしたか
/*
    public static Action<int,bool> OnUseOtoFudaCard; 
*/

    //プレイヤーのタップ等の行動を格納
    public PlayerMovement[] _playerMovement;

    public int[,] CacheNoteCounters => _cacheNoteCounters;


    private void Start()
    {
    }

    public override void Init(AudioSource audioSource,
        JudgeProfile profile, JudgeTextController judgeTextController,
        PlayerManager playerManager,
        TimingInformationList timingInformationList,
        List<NoteTimingInformation>[] currentStateTimingInformation,
        int[,] noteCounters)
    {
        base.Init(audioSource,
            profile, judgeTextController,
            playerManager,
            timingInformationList, currentStateTimingInformation,
            noteCounters);

        keyBeamController.Init();
        //レーンライトの表示をオフ
        keyBeamController.BeamOffAll();

        //それぞれのロングノーツのフラグをオフにする
        for (var i = 0; i < isLongNoteStart.Length; i++)
        {
            isLongNoteStart[i] = false;
        }

        /*
        //各難易度の各レーンのノーツ情報を初期化する
        for (var i = 0; i < (int) PlayerFumenState.End; i++)
        {
            for (var k = 0; k < 5; k++)
            {
                NoteCounters[i, k] = 0;
            }
        }
        */

        //PlayerManagerをインスタンス化
        //各種ムーブメントを初期化
        foreach (var t in _playerMovement)
        {
            t.InitMovement(this);
        }
    }

    private void OnEnable()
    {
        FumenFlowManager.OnMusicStart += OnMusicStart;
    }


    private void OnMusicStart(int _playerID)
    {
        if (_playerID == playerID)
        {
            isStartMusic = true;
        }
    }


    public void KeyInputUpdate()
    {
/*        Debug.Log(_playerManager._players[playerID].FumenState);*/

        //Debug.Log(noteCounters[1, 3]);
        //プレイヤーの行動(入力)をチェックする

        if (isStartMusic)
        {
        }

        for (var i = 0; i < _playerMovement.Length; i++)
        {
            if (_cacheCurrentTimings != null)
            {
                _playerMovement[i].PlayerMovementCheck(_audioSource.time, _cacheCurrentTimings);
            }
        }

        /*
        //todo ノーツの通過を監視するのはKeyInputManagerの仕事ではなさそう
        for (var i = 0; i < 5; i++)
        {
            //全難易度分の譜面の通過を監視する。
            FumenPassCheck(_cacheDefaultTimings[i], PlayerFumenState.DEFAULT, i);
            FumenPassCheck(_cacheMoreDifficultTimings[i], PlayerFumenState.MORE_DIFFICULT, i);
            FumenPassCheck(_cacheMoreEasyTimings[i], PlayerFumenState.MORE_EASY, i);
        }
        */

        //       Debug.Log("PassCheck");
    }

    public void ComboUp(Judge judge)
    {
        JudgeController.ComboUp(judge);
    }

    public void ComboCut(Judge judge)
    {
        JudgeController.ComboCut(judge);
    }


    /*private void FumenPassCheck(List<NoteTimingInformation> targetTimings,
        PlayerFumenState targetState, int index)
    {
        var stateIndex = (int) targetState;

        //targetTimingsの中身が空であればそのレーンにノーツは存在していない
        if (targetTimings.Count == 0)
        {
            return;
        }

        //ノーツのカウントとタイミングのカウンターが一緒であればそのレーンにノーツは存在していない
        if (targetTimings.Count == noteCounters[stateIndex, index])
        {
            return;
        }

        if (targetTimings[noteCounters[stateIndex, index]]._reachTime - _audioSource.time <
            -judgeProfile.badThreshold)
        {
            if (targetTimings[noteCounters[stateIndex, index]]._noteType == 1 ||
                targetTimings[noteCounters[stateIndex, index]]._noteType == 2 ||
                targetTimings[noteCounters[stateIndex, index]]._noteType == 3 ||
                targetTimings[noteCounters[stateIndex, index]]._noteType == 4 ||
                targetTimings[noteCounters[stateIndex, index]]._noteType == 5)
            {
                //引数で渡したStateが現在のプレイヤーのステートと同じであればMissの判定をする
                //かつ、未だアクティブなノーツだった場合。(ロングなど、終点ノーツをもつやつ)
                if (_playerManager._players[playerID].FumenState == targetState &&
                    targetTimings[noteCounters[stateIndex, index]]._noteEntity.IsActive)
                {
                    var prevValue = _playerManager._players[playerID].playerHp;
                    var currentValue = Mathf.Clamp(prevValue - 5, 0,
                        _playerManager._players[playerID].playerMaxHp);
                    var slider = _playerManager._players[playerID].playerHPSlider;

                    _playerManager._players[playerID].playerHp = currentValue;
                    slider.value = currentValue;

                    ComboCut();
                    judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                }

                //音札ノーツをスルーしたとき、譜面のステートをデフォに戻す
                if (targetTimings[noteCounters[stateIndex, index]]._noteType == 5)
                {
                    if (_playerManager._players[playerID].FumenState == targetState)
                    {
                        _playerManager._players[playerID].FumenState = PlayerFumenState.DEFAULT;
                    }
                }

                //ミスしたノーツをDeactivateする。この中で終点ノーツもDeactivateされる
                targetTimings[noteCounters[stateIndex, index]]._noteEntity.Deactivate();
                noteCounters[stateIndex, index]++;
            }


/*                        //エラー回避、2レーン目のノーツ数が最大値だったらreturn
                        Debug.LogWarning(_noteCounters[stateIndex, 2]);

                        if (_noteCounters[stateIndex, 2] >= targetTimings.Count)
                        {
                            return;
                        }#1#

/*                        //次のノーツが音札ノーツであればターンチェックのコルーチンを走らせ始める
                        if (targetTimings[_noteCounters[stateIndex, 2]].noteType == 5)
                        {
                            _playerManager.runCoroutine();
                        }#1#
        }

        //}
    }*/
}