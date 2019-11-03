using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;

public class PlayerKeyInpuManager : MonoBehaviour
{
    public JudgeProfile judgeProfile;
    internal bool isStartMusic = false;
        
    [Range(0,1)]
    public int playerID;
    public KeyCode[] playerKeys = new KeyCode[5];

    //判定を表示する用のテキスト
    public Animator[] judgeTextAnimators;

    //判定をenumで管理
    public enum Judge
    {
        Perfect = 0,
        Good = 1,
        Bad = 2,
        Miss = 3,
        None = 4
    }

    internal FumenDataManager _fumenDataManager; 
    internal AudioSource _audioSource;

    internal int[,] _noteCounters;
    //各レーンのノーツ数を格納する配列
/*
        private int[] noteCount = new int[5];
        private int[] moreDifficulNoteCount = new int[5];
        private int[] moreEasyNoteCount = new int[5];
*/
    
    public GameObject[] laneLight;
    
    //ロングノーツの開始をチェックしておくbool
    internal bool[] isLongNoteStart = new bool[5];
        
    //カードを使うときのアクション
    //パフェだったかのがしたか
    public static Action<int,bool> OnUseOtoFudaCard; 
    
    //プレイヤー情報を格納
    internal PlayerManager _playerManager;
    //プレイヤーのタップ等の行動を格納
    public PlayerMovement[] _playerMovement;
    
    
    private void Start()
    {
        //PlayerManagerをインスタンス化
        _playerManager = PlayerManager.Instance;
        _noteCounters = _playerManager._players[playerID].noteCounters;
        
        _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];
        _fumenDataManager = FumenDataManager.Instance;

        if (judgeProfile == null)
        {
            judgeProfile = ScriptableObject.CreateInstance<JudgeProfile>();
            judgeProfile.perfectThreshold = 0.15f;
            judgeProfile.goodThreshold = 0.2f;
            judgeProfile.badThreshold = 0.6f;
        }

        //各種ムーブメントを初期化
        foreach (var t in _playerMovement)
        {
            t.InitMovement(this);
        }
        
        //レーンライトの表示をオフ
        foreach (var t in laneLight)
        {
            t.SetActive(false);
        }
        
        //それぞれのロングノーツの表示をオフにする
        for (var i = 0; i < isLongNoteStart.Length; i++)
        {
            isLongNoteStart[i] = false;
        }
        
        //各難易度の各レーンのノーツ情報を初期化する
        for (var i = 0; i < 3; i++)
        {
            for (var k = 0; k < 5; k++)
            {
                _noteCounters[i, k] = 0;
            }
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


    private void OnGUI()
    {
        //プレイヤーの行動(入力)をチェックする
        foreach (var t in _playerMovement)
        {
            t.PlayerMovementCheck();
        }
        
        //        Debug.Log("Movement");
        
        for (var i = 0; i < 5; i++)
        {
            fumenPassChecker(_fumenDataManager.timings[playerID, i], PlayerFumenState.DEFAULT, i);
            fumenPassChecker(_fumenDataManager.moreDifficultTimings[playerID, i], PlayerFumenState.MORE_DIFFICULT, i);
        } 
        //       Debug.Log("PassCheck");

    }

    private void fumenPassChecker(List<FumenDataManager.NoteTimingInfomation> targetTimings,
        PlayerFumenState targetState, int index)
    {
        var stateIndex = (int) targetState;
        //もうリストがなくなりきってたらはじく
        if ((stateIndex == 1 && _fumenDataManager.mainNotes[playerID].Count == 0) ||
            (stateIndex == 2 && _fumenDataManager.moreDifficultNotes[playerID].Count == 0))
        { 
//            Debug.Log("はじいたよ");
            return;
        }

        for (int k = 0; k < targetTimings.Count; k++)
        {
            if (_noteCounters[stateIndex, index] != targetTimings.Count)
            {
                if (targetTimings[_noteCounters[stateIndex, index]].reachTime -_audioSource.time < -judgeProfile.badThreshold)
                {
                    if (_playerManager._players[playerID].FumenState == targetState)
                    {
                        //Debug.Log(" aaa :"+ (targetTimings[_noteCounters[stateIndex, index]].reachTime - _audioSource.time));
                        judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                        //Debug.LogError("Miss" + (int) Judge.Miss);
                    }

                    if (targetTimings[_noteCounters[stateIndex, index]].noteType == 5)
                    {
                        OnUseOtoFudaCard?.Invoke(playerID, false);
                    }

                    //ロングノーツの場合、始点をミスしたら終点もミス扱いにする
                    if (targetTimings[_noteCounters[stateIndex, index]].noteType == 2)
                    {
                        //2ノーツ分カウンターを進める
                        _noteCounters[stateIndex, index] += 2;
                        _playerManager._players[playerID].noteSimpleCount += 2;


                        //通過に応じてRemove
                        if (stateIndex == 1)
                        {
                            _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                            _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                        }
                        else if (stateIndex == 2)
                        {
                            _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                            _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                        }

                        if (_playerManager._players[playerID].FumenState == targetState)
                        {
                            _playerManager._players[playerID].playerHp -= 5;
                            var slider = _playerManager._players[playerID].playerHPSlider;
                            slider.value = Mathf.Clamp(_playerManager._players[playerID].playerHp, 0,
                                slider.maxValue);


                            judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                            //Debug.LogError("Miss");
                        }

                    }
                    else
                    {
                        _noteCounters[stateIndex, index]++;
                        _playerManager._players[playerID].noteSimpleCount++;

                        //引数で渡したStateが現在のプレイヤーのステートと同じであればMissの判定をする。
                        if (_playerManager._players[playerID].FumenState == targetState)
                        {
                            _playerManager._players[playerID].playerHp -= 5;
                            var slider = _playerManager._players[playerID].playerHPSlider;
                            slider.value = Mathf.Clamp(_playerManager._players[playerID].playerHp, 0,
                                slider.maxValue);


                            judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                            // Debug.LogError(_playerManager._players[playerID].FumenState +"_______"+targetState);
                        }

                        //通過に応じてRemove
                        if (stateIndex == 1)
                        {
                            _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                        }
                        else if (stateIndex == 2)
                        {
                            _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                        }
                    }

/*                        //エラー回避、2レーン目のノーツ数が最大値だったらreturn
                        Debug.LogWarning(_noteCounters[stateIndex, 2]);

                        if (_noteCounters[stateIndex, 2] >= targetTimings.Count)
                        {
                            return;
                        }*/

/*                        //次のノーツが音札ノーツであればターンチェックのコルーチンを走らせ始める
                        if (targetTimings[_noteCounters[stateIndex, 2]].noteType == 5)
                        {
                            _playerManager.runCoroutine();
                        }*/
                }
            }
        }

    }




}
