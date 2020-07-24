using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKeyInputManager : MonoBehaviour
{
    public JudgeProfile judgeProfile;
    internal bool isStartMusic = false;

    [Range(0, 1)] public int playerID;
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
/*
    public static Action<int,bool> OnUseOtoFudaCard; 
*/

    //プレイヤー情報を格納
    internal PlayerManager _playerManager;

    //プレイヤーのタップ等の行動を格納
    public PlayerMovement[] _playerMovement;

    //コンボカウンター用
    private ComboCounter _comboCounter;
    [SerializeField] private Text comboCountText;


    //ノーツ情報
    public int[,] noteCounters = new int[3, 5];
    public int noteSimpleCount = 0;

    private void Start()
    {
        _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];

        //PlayerManagerをインスタンス化
        _playerManager = PlayerManager.Instance;

        _fumenDataManager = FumenDataManager.Instance;

        if (judgeProfile == null)
        {
            judgeProfile = ScriptableObject.CreateInstance<JudgeProfile>();
            judgeProfile.perfectThreshold = 0.15f;
            judgeProfile.goodThreshold = 0.2f;
            judgeProfile.badThreshold = 0.6f;
        }

        if (comboCountText != null)
        {
            _comboCounter = new ComboCounter(comboCountText,
                judgeProfile.comboInteractionScale, judgeProfile.comboInteractionTime);
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

        //それぞれのロングノーツのフラグをオフにする
        for (var i = 0; i < isLongNoteStart.Length; i++)
        {
            isLongNoteStart[i] = false;
        }
    }

    public void Init()
    {
        //レーンライトの表示をオフ
        foreach (var t in laneLight)
        {
            t.SetActive(false);
        }

        //それぞれのロングノーツのフラグをオフにする
        for (var i = 0; i < isLongNoteStart.Length; i++)
        {
            isLongNoteStart[i] = false;
        }

        //各難易度の各レーンのノーツ情報を初期化する
        for (var i = 0; i < (int) PlayerFumenState.End; i++)
        {
            for (var k = 0; k < 5; k++)
            {
                noteCounters[i, k] = 0;
            }
        }

        noteSimpleCount = 0;
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


    private void Update()
    {
/*        Debug.Log(_playerManager._players[playerID].FumenState);*/

        //Debug.Log(noteCounters[1, 3]);
        //プレイヤーの行動(入力)をチェックする
        foreach (var t in _playerMovement)
        {
            t.PlayerMovementCheck();
        }

        //        Debug.Log("Movement");

        for (var i = 0; i < 5; i++)
        {
            FumenPassCheck(_fumenDataManager.timings[playerID, i], PlayerFumenState.DEFAULT, i);
            FumenPassCheck(_fumenDataManager.moreDifficultTimings[playerID, i], PlayerFumenState.MORE_DIFFICULT, i);
        }

        //       Debug.Log("PassCheck");
    }

    public void ComboUp()
    {
        _comboCounter?.ComboUp();
    }

    public void ComboCut()
    {
        _comboCounter?.ComboCut();
    }


    private void FumenPassCheck(List<FumenDataManager.NoteTimingInformation> targetTimings,
        PlayerFumenState targetState, int index)
    {
        var stateIndex = (int) targetState;
        //もうリストがなくなりきってたらはじく
        if ((stateIndex == 1 && _fumenDataManager.mainNotes[playerID].Count == 0) ||
            (stateIndex == 2 && _fumenDataManager.moreDifficultNotes[playerID].Count == 0))
        {
            return;
        }

        for (int k = 0; k < targetTimings.Count; k++)
        {
            if (noteCounters[stateIndex, index] < targetTimings.Count)
            {
/*
                Debug.Log((PlayerFumenState) (stateIndex) + ":" + _noteCounters[stateIndex, index]);
*/
                if (targetTimings[noteCounters[stateIndex, index]].reachTime - _audioSource.time <
                    -judgeProfile.badThreshold)
                {
/*                    if (_playerManager._players[playerID].FumenState == targetState)
                    {
                        //Debug.Log(" aaa :"+ (targetTimings[_noteCounters[stateIndex, index]].reachTime - _audioSource.time));
                        judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                        //Debug.LogError("Miss" + (int) Judge.Miss);
                    }*/


                    //ロングノーツの場合、始点をミスしたら終点もミス扱いにする
                    if (targetTimings[noteCounters[stateIndex, index]].noteType == 2)
                    {
                        //通過に応じてRemove
                        if (stateIndex == 1)
                        {
                            _fumenDataManager.mainNotes[playerID][0].DeleteNote();
                            _fumenDataManager.mainNotes[playerID].RemoveAt(0);

                            _fumenDataManager.mainNotes[playerID][0].DeleteNote();
                            _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                        }
                        else if (stateIndex == 2)
                        {
                            _fumenDataManager.moreDifficultNotes[playerID][0].DeleteNote();
                            _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);

                            _fumenDataManager.moreDifficultNotes[playerID][0].DeleteNote();
                            _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                        }

                        if (_playerManager._players[playerID].FumenState == targetState)
                        {
                            var prevValue = _playerManager._players[playerID].playerHp;
                            var currentValue = Mathf.Clamp(prevValue - 5, 0,
                                _playerManager._players[playerID].playerMaxHp);
                            var slider = _playerManager._players[playerID].playerHPSlider;

                            _playerManager._players[playerID].playerHp = currentValue;
                            slider.value = currentValue;

                            ComboCut();
                            judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                            //Debug.LogError("Miss");
                        }

                        //音札ノーツをスルーしたとき、譜面のステートをデフォに戻す
                        if (targetTimings[noteCounters[stateIndex, index]].noteType == 5)
                        {
                            if (_playerManager._players[playerID].FumenState == targetState)
                            {
                                _playerManager._players[playerID].FumenState = PlayerFumenState.DEFAULT;
                            }
                        }

                        //2ノーツ分カウンターを進める
                        noteCounters[stateIndex, index] += 2;
                        noteSimpleCount += 2;
//                        Debug.Log("long");
                    }
                    else
                    {
                        //引数で渡したStateが現在のプレイヤーのステートと同じであればMissの判定をする。
                        if (_playerManager._players[playerID].FumenState == targetState)
                        {
//                            Debug.Log("State : " + _playerManager._players[playerID].FumenState);

                            var prevValue = _playerManager._players[playerID].playerHp;
                            var currentValue = Mathf.Clamp(prevValue - 5, 0,
                                _playerManager._players[playerID].playerMaxHp);
                            var slider = _playerManager._players[playerID].playerHPSlider;

                            _playerManager._players[playerID].playerHp = currentValue;
                            slider.value = currentValue;

                            ComboCut();
                            judgeTextAnimators[(int) Judge.Miss].Play("Judge", 0, 0.0f);
                            // Debug.LogError(_playerManager._players[playerID].FumenState +"_______"+targetState);
                        }

                        //通過に応じてRemove
                        if (stateIndex == 1)
                        {
                            _fumenDataManager.mainNotes[playerID][0].DeleteNote();
                            _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                        }
                        else if (stateIndex == 2)
                        {
                            _fumenDataManager.moreDifficultNotes[playerID][0].DeleteNote();
                            _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                        }

                        //音札ノーツをスルーしたとき、譜面のステートをデフォに戻す
                        if (targetTimings[noteCounters[stateIndex, index]].noteType == 5)
                        {
                            if (_playerManager._players[playerID].FumenState == targetState)
                            {
                                _playerManager._players[playerID].FumenState = PlayerFumenState.DEFAULT;
                            }
                        }

                        noteCounters[stateIndex, index]++;
                        noteSimpleCount++;
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